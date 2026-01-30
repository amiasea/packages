using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    // ========================================================================
    //  PROJECTION ENGINE — UNIFIED #1 + #2 (C# 7.3 COMPATIBLE)
    // ========================================================================

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
            ProjectionField field)
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
            IProjectionEnumType enumType = type as IProjectionEnumType;
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

    // ========================================================================
    //  PUBLIC SURFACE
    // ========================================================================

    public sealed class ProjectionRequest
    {
        public string RootTypeName { get; private set; }
        public IReadOnlyList<ProjectionField> Fields { get; private set; }
        public ProjectionContext Context { get; private set; }

        public ProjectionRequest(
            string rootTypeName,
            IReadOnlyList<ProjectionField> fields,
            ProjectionContext context)
        {
            if (rootTypeName == null) throw new ArgumentNullException(nameof(rootTypeName));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (context == null) throw new ArgumentNullException(nameof(context));

            RootTypeName = rootTypeName;
            Fields = fields;
            Context = context;
        }
    }

    public sealed class ProjectionField
    {
        public string Name { get; private set; }
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Arguments { get; private set; }
        public IReadOnlyList<ProjectionField> Children { get; private set; }
        public string Alias { get; private set; }
        public IReadOnlyList<ProjectionDirective> Directives { get; private set; }

        public ProjectionField(
            string name,
            IReadOnlyDictionary<string, ProjectionArgumentValue> arguments,
            IReadOnlyList<ProjectionField> children,
            string alias,
            IReadOnlyList<ProjectionDirective> directives)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (children == null) throw new ArgumentNullException(nameof(children));

            Name = name;
            Arguments = arguments;
            Children = children;
            Alias = alias;
            Directives = directives ?? new List<ProjectionDirective>();
        }
    }

    public sealed class ProjectionDirective
    {
        public string Name { get; private set; }
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Arguments { get; private set; }

        public ProjectionDirective(
            string name,
            IReadOnlyDictionary<string, ProjectionArgumentValue> arguments)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            Name = name;
            Arguments = arguments;
        }
    }

    public sealed class ProjectionContext
    {
        public IReadOnlyDictionary<string, object> Items { get; private set; }

        public ProjectionContext(IReadOnlyDictionary<string, object> items)
        {
            Items = items ?? new Dictionary<string, object>();
        }
    }

    // ========================================================================
    //  ARGUMENT VALUE MODEL
    // ========================================================================

    public abstract class ProjectionArgumentValue
    {
    }

    public sealed class ProjectionScalarValue : ProjectionArgumentValue
    {
        public object Raw { get; private set; }

        public ProjectionScalarValue(object raw)
        {
            Raw = raw;
        }
    }

    public sealed class ProjectionEnumValue : ProjectionArgumentValue
    {
        public string Raw { get; private set; }

        public ProjectionEnumValue(string raw)
        {
            if (raw == null) throw new ArgumentNullException(nameof(raw));
            Raw = raw;
        }
    }

    public sealed class ProjectionListValue : ProjectionArgumentValue
    {
        public IReadOnlyList<ProjectionArgumentValue> Elements { get; private set; }

        public ProjectionListValue(IReadOnlyList<ProjectionArgumentValue> elements)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            Elements = elements;
        }
    }

    public sealed class ProjectionObjectValue : ProjectionArgumentValue
    {
        public IReadOnlyDictionary<string, ProjectionArgumentValue> Fields { get; private set; }

        public ProjectionObjectValue(IReadOnlyDictionary<string, ProjectionArgumentValue> fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            Fields = fields;
        }
    }

    public sealed class ProjectionVariableReference : ProjectionArgumentValue
    {
        public string Name { get; private set; }

        public ProjectionVariableReference(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }

    public sealed class ProjectionResult
    {
        public object Data { get; private set; }
        public IReadOnlyList<ProjectionError> Errors { get; private set; }

        public ProjectionResult(object data, IReadOnlyList<ProjectionError> errors)
        {
            Data = data;
            Errors = errors ?? new List<ProjectionError>();
        }
    }

    public sealed class ProjectionError
    {
        public string Message { get; private set; }
        public IReadOnlyList<string> Path { get; private set; }

        public ProjectionError(string message, IReadOnlyList<string> path)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            Message = message;
            Path = path ?? new List<string>();
        }
    }

    // ========================================================================
    //  NORMALIZED MODEL
    // ========================================================================

    public sealed class NormalizedProjectionRequest
    {
        public IProjectionType RootType { get; private set; }
        public IReadOnlyList<NormalizedProjectionField> Fields { get; private set; }
        public ProjectionContext Context { get; private set; }

        public NormalizedProjectionRequest(
            IProjectionType rootType,
            IReadOnlyList<NormalizedProjectionField> fields,
            ProjectionContext context)
        {
            if (rootType == null) throw new ArgumentNullException(nameof(rootType));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (context == null) throw new ArgumentNullException(nameof(context));

            RootType = rootType;
            Fields = fields;
            Context = context;
        }
    }

    public sealed class NormalizedProjectionField
    {
        public IProjectionFieldDefinition Definition { get; private set; }
        public IReadOnlyDictionary<string, NormalizedArgumentValue> Arguments { get; private set; }
        public IReadOnlyList<NormalizedProjectionField> Children { get; private set; }
        public string Alias { get; private set; }
        public IReadOnlyList<ProjectionDirective> Directives { get; private set; }

        public NormalizedProjectionField(
            IProjectionFieldDefinition definition,
            IReadOnlyDictionary<string, NormalizedArgumentValue> arguments,
            IReadOnlyList<NormalizedProjectionField> children,
            string alias,
            IReadOnlyList<ProjectionDirective> directives)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (children == null) throw new ArgumentNullException(nameof(children));

            Definition = definition;
            Arguments = arguments;
            Children = children;
            Alias = alias;
            Directives = directives ?? new List<ProjectionDirective>();
        }
    }

    public sealed class NormalizedArgumentValue
    {
        public NormalizedArgumentKind Kind { get; private set; }
        public object Value { get; private set; }
        public IProjectionInputType Type { get; private set; }

        private NormalizedArgumentValue(
            NormalizedArgumentKind kind,
            object value,
            IProjectionInputType type)
        {
            Kind = kind;
            Value = value;
            Type = type;
        }

        public static NormalizedArgumentValue Scalar(object value, IProjectionScalarType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return new NormalizedArgumentValue(NormalizedArgumentKind.Scalar, value, type);
        }

        public static NormalizedArgumentValue Enum(object value, IProjectionEnumType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return new NormalizedArgumentValue(NormalizedArgumentKind.Enum, value, type);
        }

        public static NormalizedArgumentValue List(
            IReadOnlyList<NormalizedArgumentValue> elements,
            IProjectionInputType elementType)
        {
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.List,
                elements,
                new ProjectionListTypeProxy(elementType));
        }

        public static NormalizedArgumentValue Object(
            IReadOnlyDictionary<string, NormalizedArgumentValue> fields,
            IProjectionInputObjectType type)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.Object,
                fields,
                type);
        }

        public static NormalizedArgumentValue Variable(
            string name,
            IProjectionInputType type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return new NormalizedArgumentValue(
                NormalizedArgumentKind.Variable,
                name,
                type);
        }
    }

    public enum NormalizedArgumentKind
    {
        Scalar,
        Enum,
        List,
        Object,
        Variable
    }

    internal sealed class ProjectionListTypeProxy : IProjectionListType
    {
        public string Name
        {
            get { return "[" + ElementType.Name + "]"; }
        }

        public IProjectionInputType ElementType { get; private set; }

        public ProjectionListTypeProxy(IProjectionInputType elementType)
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            ElementType = elementType;
        }
    }

    // ========================================================================
    //  COMPARISON INPUT MODEL (#2)
    // ========================================================================

    public sealed class ComparisonInput
    {
        public ProjectionArgumentValue Eq { get; private set; }
        public ProjectionArgumentValue Ne { get; private set; }
        public ProjectionArgumentValue Gt { get; private set; }
        public ProjectionArgumentValue Gte { get; private set; }
        public ProjectionArgumentValue Lt { get; private set; }
        public ProjectionArgumentValue Lte { get; private set; }
        public ProjectionArgumentValue Like { get; private set; }
        public ProjectionArgumentValue Between { get; private set; }

        public ComparisonInput(
            ProjectionArgumentValue eq,
            ProjectionArgumentValue ne,
            ProjectionArgumentValue gt,
            ProjectionArgumentValue gte,
            ProjectionArgumentValue lt,
            ProjectionArgumentValue lte,
            ProjectionArgumentValue like,
            ProjectionArgumentValue between)
        {
            Eq = eq;
            Ne = ne;
            Gt = gt;
            Gte = gte;
            Lt = lt;
            Lte = lte;
            Like = like;
            Between = between;
        }
    }

    public interface IComparisonInputType : IProjectionInputObjectType
    {
    }

    public sealed class ComparisonInputType : IComparisonInputType
    {
        public string Name
        {
            get { return "comparison"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public ComparisonInputType(IProjectionInputType scalarType)
        {
            if (scalarType == null) throw new ArgumentNullException(nameof(scalarType));

            Fields = new IProjectionInputFieldDefinition[]
            {
                new ComparisonField("eq", scalarType),
                new ComparisonField("ne", scalarType),
                new ComparisonField("gt", scalarType),
                new ComparisonField("gte", scalarType),
                new ComparisonField("lt", scalarType),
                new ComparisonField("lte", scalarType),
                new ComparisonField("like", scalarType),
                new ComparisonField("between", new ComparisonBetweenListType(scalarType))
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("comparison input does not allow extra fields");
        }

        private sealed class ComparisonField : IProjectionInputFieldDefinition
        {
            public string Name { get; private set; }
            public IProjectionInputType Type { get; private set; }
            public bool IsNonNull
            {
                get { return false; }
            }

            public ProjectionArgumentValue DefaultValue
            {
                get { return null; }
            }

            public ComparisonField(string name, IProjectionInputType type)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
            }
        }

        private sealed class ComparisonBetweenListType : IProjectionListType
        {
            public string Name
            {
                get { return "[" + ElementType.Name + "]"; }
            }

            public IProjectionInputType ElementType { get; private set; }

            public ComparisonBetweenListType(IProjectionInputType elementType)
            {
                if (elementType == null) throw new ArgumentNullException(nameof(elementType));
                ElementType = elementType;
            }
        }
    }

    // ========================================================================
    //  ORDER BY INPUT MODEL (#2)
    // ========================================================================

    public sealed class OrderByInput
    {
        public string Field { get; private set; }
        public string Direction { get; private set; }

        public OrderByInput(string field, string direction)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (direction == null) throw new ArgumentNullException(nameof(direction));

            Field = field;
            Direction = direction;
        }
    }

    public interface IOrderByInputType : IProjectionInputObjectType
    {
    }

    public sealed class OrderByInputType : IOrderByInputType
    {
        public string Name
        {
            get { return "order_by"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public OrderByInputType()
        {
            Fields = new IProjectionInputFieldDefinition[]
            {
                new OrderByField("field", new StringScalarType(), true),
                new OrderByField("direction", new StringScalarType(), true)
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("order_by does not allow extra fields");
        }

        private sealed class OrderByField : IProjectionInputFieldDefinition
        {
            public string Name { get; private set; }
            public IProjectionInputType Type { get; private set; }
            public bool IsNonNull { get; private set; }
            public ProjectionArgumentValue DefaultValue
            {
                get { return null; }
            }

            public OrderByField(string name, IProjectionInputType type, bool isNonNull)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
                IsNonNull = isNonNull;
            }
        }
    }

    // ========================================================================
    //  PAGING INPUT MODEL (#2)
    // ========================================================================

    public sealed class PagingInput
    {
        public ProjectionArgumentValue Limit { get; private set; }
        public ProjectionArgumentValue Offset { get; private set; }
        public IReadOnlyList<OrderByInput> OrderBy { get; private set; }

        public PagingInput(
            ProjectionArgumentValue limit,
            ProjectionArgumentValue offset,
            IReadOnlyList<OrderByInput> orderBy)
        {
            Limit = limit;
            Offset = offset;
            OrderBy = orderBy;
        }
    }

    public interface IPagingInputType : IProjectionInputObjectType
    {
    }

    public sealed class PagingInputType : IPagingInputType
    {
        public string Name
        {
            get { return "paging"; }
        }

        public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }

        public bool AllowExtraFields
        {
            get { return false; }
        }

        public PagingInputType(
            IProjectionInputType intType,
            IOrderByInputType orderByType)
        {
            if (intType == null) throw new ArgumentNullException(nameof(intType));
            if (orderByType == null) throw new ArgumentNullException(nameof(orderByType));

            Fields = new IProjectionInputFieldDefinition[]
            {
                new PagingField("limit", intType),
                new PagingField("offset", intType),
                new PagingField("order_by", new OrderByListType(orderByType))
            };
        }

        public IProjectionInputType InferExtraFieldType(string name, ProjectionArgumentValue rawValue)
        {
            throw new InvalidOperationException("paging does not allow extra fields");
        }

        private sealed class PagingField : IProjectionInputFieldDefinition
        {
            public string Name { get; private set; }
            public IProjectionInputType Type { get; private set; }
            public bool IsNonNull
            {
                get { return false; }
            }

            public ProjectionArgumentValue DefaultValue
            {
                get { return null; }
            }

            public PagingField(string name, IProjectionInputType type)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (type == null) throw new ArgumentNullException(nameof(type));

                Name = name;
                Type = type;
            }
        }

        private sealed class OrderByListType : IProjectionListType
        {
            public string Name
            {
                get { return "[" + ElementType.Name + "]"; }
            }

            public IProjectionInputType ElementType { get; private set; }

            public OrderByListType(IProjectionInputType elementType)
            {
                if (elementType == null) throw new ArgumentNullException(nameof(elementType));
                ElementType = elementType;
            }
        }
    }

    // ========================================================================
    //  BASIC STRING SCALAR (placeholder)
    // ========================================================================

    internal sealed class StringScalarType : IProjectionScalarType
    {
        public string Name
        {
            get { return "String"; }
        }

        public object Coerce(object raw)
        {
            return raw == null ? null : raw.ToString();
        }
    }


    // ========================================================================
    //  PLANNING AND EXECUTION
    // ========================================================================

    public interface IProjectionPlanner
    {
        ProjectionPlan Plan(
            NormalizedProjectionRequest request,
            CancellationToken cancellationToken);
    }

    public interface IProjectionExecutor
    {
        Task<ProjectionResult> ExecuteAsync(
            ProjectionPlan plan,
            CancellationToken cancellationToken);
    }

    public sealed class ProjectionPlan
    {
        public object RootExecutionNode { get; private set; }

        public ProjectionPlan(object rootExecutionNode)
        {
            RootExecutionNode = rootExecutionNode;
        }
    }
}