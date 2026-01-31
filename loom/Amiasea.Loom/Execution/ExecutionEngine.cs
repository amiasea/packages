using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.AST;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public sealed class ExecutionEngine : IExecutionEngine
    {
        private readonly IProjectionSchema _schema;
        private readonly IProjectionPlanner _planner;
        private readonly IProjectionExecutor _executor;

        public ExecutionEngine(
            IProjectionSchema schema,
            IProjectionPlanner planner,
            IProjectionExecutor executor)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            if (planner == null) throw new ArgumentNullException(nameof(planner));
            if (executor == null) throw new ArgumentNullException(nameof(executor));

            _schema = schema;
            _planner = planner;
            _executor = executor;
        }

        public Task<ProjectionResult> ExecuteAsync(
            ProjectionRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            NormalizedProjectionRequest normalized = NormalizeRequest(request);
            ProjectionPlan plan = _planner.Plan(normalized, cancellationToken);
            return _executor.ExecuteAsync(plan, cancellationToken);
        }

        // ====================================================================
        //  REQUEST NORMALIZATION
        // ====================================================================

        private NormalizedProjectionRequest NormalizeRequest(ProjectionRequest request)
        {
            IProjectionType rootType = _schema.GetRootType(request.RootTypeName);
            if (rootType == null)
            {
                throw new InvalidOperationException(
                    "Unknown root type '" + request.RootTypeName + "'.");
            }

            var normalizedFields = request.Fields
                .Select(f => NormalizeField(rootType, f))
                .ToImmutableArray();

            return new NormalizedProjectionRequest(
                rootType,
                normalizedFields,
                request.Context);
        }

        private NormalizedProjectionField NormalizeField(
            IProjectionType parentType,
            ProjectionFieldNode field)
        {
            IProjectionFieldDefinition fieldDef = parentType.GetField(field.Name);
            if (fieldDef == null)
            {
                throw new InvalidOperationException(
                    "Unknown field '" + field.Name + "' on type '" + parentType.Name + "'.");
            }

            ImmutableDictionary<string, NormalizedArgumentValue> normalizedArgs =
                NormalizeArguments(fieldDef, field.Arguments);

            var normalizedChildren = field.Children
                .Select(child => NormalizeField(_schema.GetTypeByName(fieldDef.ReturnType.Name), child))
                .ToImmutableArray();

            return new NormalizedProjectionField(
                fieldDef,
                normalizedArgs,
                normalizedChildren,
                field.Alias,
                field.Directives ?? new List<ProjectionDirective>());
        }

        // ====================================================================
        //  ARGUMENT NORMALIZATION (ENGINE OWNS THIS)
        // ====================================================================

        private ImmutableDictionary<string, NormalizedArgumentValue> NormalizeArguments(
            IProjectionFieldDefinition fieldDef,
            IReadOnlyDictionary<string, ProjectionArgumentValue> rawArgs)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, NormalizedArgumentValue>();

            foreach (IProjectionArgumentDefinition argDef in fieldDef.Arguments)
            {
                ProjectionArgumentValue rawValue;
                rawArgs.TryGetValue(argDef.Name, out rawValue);

                ProjectionArgumentValue valueToUse = rawValue ?? argDef.DefaultValue;

                if (valueToUse == null)
                {
                    if (argDef.IsNonNull)
                    {
                        throw new InvalidOperationException(
                            "Non-null argument '" + argDef.Name + "' missing for field '" + fieldDef.Name + "'.");
                    }

                    continue;
                }

                NormalizedArgumentValue normalized = NormalizeArgumentValue(argDef.Type, valueToUse);
                builder[argDef.Name] = normalized;
            }

            if (fieldDef.AllowExtraArguments)
            {
                foreach (KeyValuePair<string, ProjectionArgumentValue> kvp in rawArgs)
                {
                    if (builder.ContainsKey(kvp.Key))
                        continue;

                    IProjectionInputType inferredType = fieldDef.InferExtraArgumentType(kvp.Key, kvp.Value);
                    NormalizedArgumentValue normalized = NormalizeArgumentValue(inferredType, kvp.Value);
                    builder[kvp.Key] = normalized;
                }
            }

            return builder.ToImmutable();
        }

        private NormalizedArgumentValue NormalizeArgumentValue(
            IProjectionInputType type,
            ProjectionArgumentValue value)
        {
            if (value is ProjectionScalarValue)
            {
                return NormalizeScalar(type, (ProjectionScalarValue)value);
            }

            if (value is ProjectionEnumValue)
            {
                return NormalizeEnum(type, (ProjectionEnumValue)value);
            }

            if (value is ProjectionListValue)
            {
                return NormalizeList(type, (ProjectionListValue)value);
            }

            if (value is ProjectionObjectValue)
            {
                return NormalizeObject(type, (ProjectionObjectValue)value);
            }

            if (value is ProjectionVariableReference)
            {
                ProjectionVariableReference variable = (ProjectionVariableReference)value;
                return NormalizedArgumentValue.Variable(variable.Name, type);
            }

            throw new InvalidOperationException(
                "Unsupported argument value '" + value.GetType().Name + "'.");
        }

        // ====================================================================
        //  SCALAR
        // ====================================================================

        private NormalizedArgumentValue NormalizeScalar(
            IProjectionInputType type,
            ProjectionScalarValue scalar)
        {
            IProjectionScalarType scalarType = type as IProjectionScalarType;
            if (scalarType == null)
            {
                throw new InvalidOperationException(
                    "Expected scalar type for '" + scalar.Raw + "', got '" + type.Name + "'.");
            }

            object coerced = scalarType.Coerce(scalar.Raw);
            return NormalizedArgumentValue.Scalar(coerced, scalarType);
        }

        // ====================================================================
        //  ENUM
        // ====================================================================

        private NormalizedArgumentValue NormalizeEnum(
            IProjectionInputType type,
            ProjectionEnumValue enumValue)
        {
            IProjectionInputEnumType enumType = type as IProjectionInputEnumType;
            if (enumType == null)
            {
                throw new InvalidOperationException(
                    "Expected enum type for '" + enumValue.Raw + "', got '" + type.Name + "'.");
            }

            object coerced = enumType.Coerce(enumValue.Raw);
            return NormalizedArgumentValue.Enum(coerced, enumType);
        }

        // ====================================================================
        //  LIST
        // ====================================================================

        private NormalizedArgumentValue NormalizeList(
            IProjectionInputType type,
            ProjectionListValue list)
        {
            IProjectionListType listType = type as IProjectionListType;
            if (listType == null)
            {
                throw new InvalidOperationException(
                    "Expected list type, got '" + type.Name + "'.");
            }

            var normalized = list.Elements
                .Select(e => NormalizeArgumentValue(listType.ElementType, e))
                .ToImmutableArray();

            return NormalizedArgumentValue.List(normalized, listType.ElementType);
        }

        // ====================================================================
        //  OBJECT (COMPARISON + PAGING INCLUDED)
        // ====================================================================

        private NormalizedArgumentValue NormalizeObject(
            IProjectionInputType type,
            ProjectionObjectValue obj)
        {
            IProjectionInputObjectType inputObjType = type as IProjectionInputObjectType;
            if (inputObjType == null)
            {
                throw new InvalidOperationException(
                    "Expected input object type, got '" + type.Name + "'.");
            }

            var builder = ImmutableDictionary.CreateBuilder<string, NormalizedArgumentValue>();

            foreach (IProjectionInputFieldDefinition fieldDef in inputObjType.Fields)
            {
                ProjectionArgumentValue rawValue;
                obj.Fields.TryGetValue(fieldDef.Name, out rawValue);

                ProjectionArgumentValue valueToUse = rawValue ?? fieldDef.DefaultValue;

                if (valueToUse == null)
                {
                    if (fieldDef.IsNonNull)
                    {
                        throw new InvalidOperationException(
                            "Non-null input field '" + fieldDef.Name + "' missing for '" + inputObjType.Name + "'.");
                    }

                    continue;
                }

                NormalizedArgumentValue normalized = NormalizeArgumentValue(fieldDef.Type, valueToUse);
                builder[fieldDef.Name] = normalized;
            }

            if (inputObjType.AllowExtraFields)
            {
                foreach (KeyValuePair<string, ProjectionArgumentValue> kvp in obj.Fields)
                {
                    if (builder.ContainsKey(kvp.Key))
                        continue;

                    IProjectionInputType inferredType = inputObjType.InferExtraFieldType(kvp.Key, kvp.Value);
                    NormalizedArgumentValue normalized = NormalizeArgumentValue(inferredType, kvp.Value);
                    builder[kvp.Key] = normalized;
                }
            }

            return NormalizedArgumentValue.Object(builder.ToImmutable(), inputObjType);
        }
    }
}