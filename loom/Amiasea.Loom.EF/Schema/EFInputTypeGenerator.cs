using System;
using System.Collections.Generic;
using Amiasea.Loom.EF.Schema;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF
{
    public sealed class EFInputTypeGenerator : IEFSchemaResolver
    {
        private readonly DbContext _db;

        public EFInputTypeGenerator(DbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            _db = db;
        }

        public void Resolve(EFSchemaContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var model = _db.Model;

            foreach (var clr in context.Names.Keys)
            {
                var entity = model.FindEntityType(clr);
                if (entity == null)
                    continue;

                var filter = BuildEntityFilter(clr, context);
                context.InputTypes[clr] = filter;
            }
        }

        private IProjectionInputType BuildEntityFilter(Type clr, EFSchemaContext context)
        {
            string typeName;
            if (!context.Names.TryGetValue(clr, out typeName))
                throw new InvalidOperationException("No schema name registered for CLR type '" + clr.FullName + "'.");

            var filterName = typeName + "Filter";

            List<IProjectionFieldDefinition> fieldList;
            if (!context.Fields.TryGetValue(clr, out fieldList))
                fieldList = new List<IProjectionFieldDefinition>();

            var inputFields = new List<IProjectionInputFieldDefinition>();

            foreach (var field in fieldList)
            {
                var inputType = ResolveFilterType(field.ReturnType, context);
                inputFields.Add(new ProjectionInputFieldDefinition(
                    field.Name,
                    inputType,
                    false,
                    null
                ));
            }

            // Logical operators are handled via extra-field resolver.
            Func<string, ProjectionArgumentValue, IProjectionInputType> resolver =
                delegate (string name, ProjectionArgumentValue raw)
                {
                    IProjectionInputType self;
                    if (!context.InputTypes.TryGetValue(clr, out self) || self == null)
                        throw new InvalidOperationException("Filter type for '" + clr.FullName + "' not initialized.");

                    if (name == "and" || name == "or")
                        return new ProjectionListInputType(self);

                    if (name == "not")
                        return self;

                    throw new InvalidOperationException("Unknown filter field '" + name + "'.");
                };

            var inputObj = new ProjectionInputObjectType(
                filterName,
                inputFields,
                true,
                resolver
            );

            return inputObj;
        }

        private IProjectionInputType ResolveFilterType(
            IProjectionOutputType outputType,
            EFSchemaContext context)
        {
            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));

            var nn = outputType as ProjectionOutputNonNullType;
            if (nn != null)
            {
                var inner = ResolveFilterType(nn.InnerType, context);
                return new ProjectionNonNullInputType(inner);
            }

            var scalarOut = outputType as ProjectionOutputScalarType;
            if (scalarOut != null)
            {
                return BuildScalarFilter(scalarOut);
            }

            var enumOut = outputType as ProjectionOutputEnumType;
            if (enumOut != null)
            {
                return BuildEnumFilter(enumOut);
            }

            var obj = outputType as ProjectionObjectType;
            if (obj != null)
            {
                // Try to treat as entity: match by schema name
                foreach (var kvp in context.Names)
                {
                    if (string.Equals(kvp.Value, obj.Name, StringComparison.Ordinal))
                    {
                        IProjectionInputType existing;
                        if (context.InputTypes.TryGetValue(kvp.Key, out existing) && existing != null)
                            return existing;
                    }
                }

                // Otherwise treat as complex type
                return BuildComplexFilter(obj, context);
            }

            var listOut = outputType as ProjectionOutputListType;
            if (listOut != null)
            {
                var element = ResolveFilterType(listOut.ElementType, context);
                return new ProjectionListInputType(element);
            }

            throw new InvalidOperationException(
                "Unsupported output type '" + outputType.GetType().Name + "' for filter generation.");
        }

        private IProjectionInputType BuildEnumFilter(ProjectionOutputEnumType enumOut)
        {
            if (enumOut == null) throw new ArgumentNullException(nameof(enumOut));

            var name = enumOut.Name + "EnumFilter";

            var fields = new List<IProjectionInputFieldDefinition>();

            var enumInput = new ProjectionInputEnumType(enumOut.Name, enumOut.Values);

            fields.Add(new ProjectionInputFieldDefinition("eq", enumInput, false, null));
            fields.Add(new ProjectionInputFieldDefinition("neq", enumInput, false, null));
            fields.Add(new ProjectionInputFieldDefinition("in", new ProjectionListInputType(enumInput), false, null));
            fields.Add(new ProjectionInputFieldDefinition("nin", new ProjectionListInputType(enumInput), false, null));

            return new ProjectionInputObjectType(name, fields, false, null);
        }

        private IProjectionInputType BuildScalarFilter(ProjectionOutputScalarType scalarOut)
        {
            if (scalarOut == null) throw new ArgumentNullException(nameof(scalarOut));

            var name = scalarOut.Name + "Filter";

            var fields = new List<IProjectionInputFieldDefinition>();

            // Input scalar type for this output scalar
            var scalarInput = new ProjectionScalarType(
                scalarOut.Name,
                delegate (object raw) { return raw; }
            );

            fields.Add(new ProjectionInputFieldDefinition("eq", scalarInput, false, null));
            fields.Add(new ProjectionInputFieldDefinition("neq", scalarInput, false, null));
            fields.Add(new ProjectionInputFieldDefinition("in", new ProjectionListInputType(scalarInput), false, null));
            fields.Add(new ProjectionInputFieldDefinition("nin", new ProjectionListInputType(scalarInput), false, null));

            var t = scalarOut.ClrType;

            if (IsNumeric(t))
            {
                fields.Add(new ProjectionInputFieldDefinition("lt", scalarInput, false, null));
                fields.Add(new ProjectionInputFieldDefinition("lte", scalarInput, false, null));
                fields.Add(new ProjectionInputFieldDefinition("gt", scalarInput, false, null));
                fields.Add(new ProjectionInputFieldDefinition("gte", scalarInput, false, null));
            }

            if (t == typeof(string))
            {
                fields.Add(new ProjectionInputFieldDefinition("contains", scalarInput, false, null));
                fields.Add(new ProjectionInputFieldDefinition("startsWith", scalarInput, false, null));
                fields.Add(new ProjectionInputFieldDefinition("endsWith", scalarInput, false, null));
            }

            return new ProjectionInputObjectType(name, fields, false, null);
        }

        private IProjectionInputType BuildComplexFilter(
            ProjectionObjectType obj,
            EFSchemaContext context)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var name = obj.Name + "Filter";

            var fields = new List<IProjectionInputFieldDefinition>();

            // Find CLR type(s) whose schema name matches this object name
            foreach (var kvp in context.Names)
            {
                if (!string.Equals(kvp.Value, obj.Name, StringComparison.Ordinal))
                    continue;

                List<IProjectionFieldDefinition> fieldList;
                if (!context.Fields.TryGetValue(kvp.Key, out fieldList))
                    continue;

                foreach (var f in fieldList)
                {
                    var input = ResolveFilterType(f.ReturnType, context);
                    fields.Add(new ProjectionInputFieldDefinition(
                        f.Name,
                        input,
                        false,
                        null
                    ));
                }
            }

            ProjectionInputObjectType complexFilter = null;

            Func<string, ProjectionArgumentValue, IProjectionInputType> resolver =
                delegate (string fieldName, ProjectionArgumentValue raw)
                {
                    if (complexFilter == null)
                        throw new InvalidOperationException("Complex filter type not initialized.");

                    if (fieldName == "and" || fieldName == "or")
                        return new ProjectionListInputType(complexFilter);

                    if (fieldName == "not")
                        return complexFilter;

                    throw new InvalidOperationException("Unknown filter field '" + fieldName + "'.");
                };

            complexFilter = new ProjectionInputObjectType(
                name,
                fields,
                true,
                resolver
            );

            return complexFilter;
        }

        private static bool IsNumeric(Type t)
        {
            return t == typeof(int) ||
                   t == typeof(long) ||
                   t == typeof(short) ||
                   t == typeof(byte) ||
                   t == typeof(float) ||
                   t == typeof(double) ||
                   t == typeof(decimal);
        }
    }
}