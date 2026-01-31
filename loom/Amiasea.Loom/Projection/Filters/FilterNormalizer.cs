using System;
using System.Collections.Generic;
using System.Linq;
using Amiasea.Loom.AST;
using Amiasea.Loom.Schema;

namespace Amiasea.Loom.Projection
{
    public sealed class FilterNormalizer : IFilterNormalizer
    {
        private readonly IProjectionSchema _schema;

        public FilterNormalizer(IProjectionSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");
            _schema = schema;
        }

        public FilterNode Normalize(ObjectValueNode ast, string rootType)
        {
            if (ast == null)
                return null;

            var ctx = new ValidationContext(rootType);
            var node = NormalizeInternal(ast, ctx);

            ctx.ThrowIfErrors();
            return node;
        }

        private FilterNode NormalizeInternal(ObjectValueNode ast, ValidationContext ctx)
        {
            FilterNode logical;
            if (TryLogical(ast, ctx, out logical))
                return logical;

            return NormalizeComparison(ast, ctx);
        }

        // ------------------------------------------------------------
        // LOGICAL
        // ------------------------------------------------------------
        private bool TryLogical(ObjectValueNode ast, ValidationContext ctx, out FilterNode node)
        {
            node = null;

            foreach (var field in ast.Fields)
            {
                if (field.Name == "and")
                {
                    node = new FilterNode();
                    node.And = NormalizeList(field, ctx);
                    return true;
                }

                if (field.Name == "or")
                {
                    node = new FilterNode();
                    node.Or = NormalizeList(field, ctx);
                    return true;
                }

                if (field.Name == "not")
                {
                    var obj = field.Value as ObjectValueNode;
                    if (obj == null)
                    {
                        ctx.AddError(field.Name, "'not' must be an object.");
                        return true;
                    }

                    node = new FilterNode();
                    node.Not = NormalizeInternal(obj, ctx);
                    return true;
                }
            }

            return false;
        }

        private List<FilterNode> NormalizeList(FieldNode field, ValidationContext ctx)
        {
            var list = field.Value as ListValueNode;
            if (list == null)
            {
                ctx.AddError(field.Name, "'" + field.Name + "' must be a list.");
                return new List<FilterNode>();
            }

            var items = new List<FilterNode>();
            foreach (var v in list.Values)
            {
                var obj = v as ObjectValueNode;
                if (obj != null)
                {
                    items.Add(NormalizeInternal(obj, ctx));
                }
                else
                {
                    ctx.AddError(field.Name, "Logical list items must be objects.");
                }
            }

            return items;
        }

        // ------------------------------------------------------------
        // COMPARISON
        // ------------------------------------------------------------
        private FilterNode NormalizeComparison(ObjectValueNode ast, ValidationContext ctx)
        {
            if (ast.Fields.Count != 1)
            {
                ctx.AddError("$", "Comparison filter must have exactly one field.");
                return null;
            }

            var field = ast.Fields[0];
            var fieldSchema = _schema.GetField(ctx.RootType, field.Name);

            if (fieldSchema == null)
            {
                ctx.AddError(field.Name, "Unknown field '" + field.Name + "'.");
                return null;
            }

            var opObj = field.Value as ObjectValueNode;
            if (opObj == null)
            {
                ctx.AddError(field.Name, "Comparison operator must be an object.");
                return null;
            }

            if (opObj.Fields.Count != 1)
            {
                ctx.AddError(field.Name, "Comparison operator object must have exactly one operator.");
                return null;
            }

            var opField = opObj.Fields[0];
            var opName = opField.Name;

            ValidateOperatorCompatibility(field.Name, opName, fieldSchema, ctx);

            var rawValue = ExtractValue(opField.Value, ctx, field.Name + "." + opName);
            ValidateValueCompatibility(field.Name, opName, rawValue, fieldSchema, ctx);

            // Track for schema-driven cross-field rules
            ctx.RegisterFieldValue(field.Name, opName, rawValue, fieldSchema);

            var node = new FilterNode();
            node.Field = field.Name;
            node.Operator = opName;
            node.Value = rawValue;
            return node;
        }

        // ------------------------------------------------------------
        // STRICT OPERATOR MATRIX
        // ------------------------------------------------------------
        private static readonly Dictionary<string, HashSet<FieldKind>> OperatorMatrix =
            new Dictionary<string, HashSet<FieldKind>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "eq",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Scalar,
                        FieldKind.Enum,
                        FieldKind.List
                    })
                },
                {
                    "neq",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Scalar,
                        FieldKind.Enum,
                        FieldKind.List
                    })
                },
                {
                    "gt",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Numeric,
                        FieldKind.Date
                    })
                },
                {
                    "gte",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Numeric,
                        FieldKind.Date
                    })
                },
                {
                    "lt",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Numeric,
                        FieldKind.Date
                    })
                },
                {
                    "lte",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Numeric,
                        FieldKind.Date
                    })
                },
                {
                    "contains",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.List,
                        FieldKind.String
                    })
                },
                {
                    "in",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Scalar,
                        FieldKind.Enum
                    })
                },
                {
                    "startsWith",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.String
                    })
                },
                {
                    "endsWith",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.String
                    })
                },
                {
                    "isNull",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Any
                    })
                },
                {
                    "notNull",
                    new HashSet<FieldKind>(new FieldKind[] {
                        FieldKind.Any
                    })
                }
            };

        private void ValidateOperatorCompatibility(string field, string op, IFieldSchema schema, ValidationContext ctx)
        {
            if (!schema.AllowedOperators.Contains(op))
            {
                ctx.AddError(field, "Operator '" + op + "' is not allowed for field '" + field + "'. Allowed: [" +
                    string.Join(", ", schema.AllowedOperators) + "]");
                return;
            }

            HashSet<FieldKind> allowedKinds;
            if (!OperatorMatrix.TryGetValue(op, out allowedKinds))
            {
                ctx.AddError(field, "Operator '" + op + "' is not recognized in strict mode.");
                return;
            }

            if (!allowedKinds.Contains(schema.Kind))
            {
                ctx.AddError(field,
                    "Operator '" + op + "' is not compatible with field '" + field +
                    "' of kind '" + schema.Kind + "'. Allowed kinds: [" +
                    string.Join(", ", allowedKinds) + "]");
            }
        }

        // ------------------------------------------------------------
        // STRICT VALUE VALIDATION + SCHEMA-DRIVEN REGISTRATION
        // ------------------------------------------------------------
        private void ValidateValueCompatibility(string field, string op, object value, IFieldSchema schema, ValidationContext ctx)
        {
            if (value == null && !schema.IsNullable && op != "isNull")
            {
                ctx.AddError(field, "Field '" + field + "' is non-nullable but received null.");
                return;
            }

            if (schema.IsEnum)
            {
                ValidateEnum(field, value, schema, ctx);
                return;
            }

            if (schema.IsList)
            {
                ValidateList(field, value, schema, ctx);
                return;
            }

            ValidateScalar(field, value, schema, ctx);

            // Generic numeric/date range pairing
            if (schema.IsRangeBoundary && schema.RangeGroup != null && schema.RangeRole != null)
                ctx.RegisterRangeBoundary(schema.RangeGroup, schema.RangeRole, field, op, value);

            // Temporal groups (createdAt/updatedAt, start/end, etc.)
            if (schema.TemporalGroup != null && schema.TemporalRole != null)
                ctx.RegisterTemporal(schema.TemporalGroup, schema.TemporalRole, field, op, value);

            // Geo groups (lat/lng, etc.)
            if (schema.GeoGroup != null && schema.GeoRole != null)
                ctx.RegisterGeo(schema.GeoGroup, schema.GeoRole, field, op, value);
        }

        private void ValidateEnum(string field, object value, IFieldSchema schema, ValidationContext ctx)
        {
            string s = value as string;
            if (s != null)
            {
                if (!schema.EnumValues.Contains(s))
                    ctx.AddError(field, "Invalid enum value '" + s + "'. Allowed: [" +
                        string.Join(", ", schema.EnumValues) + "]");
                return;
            }

            var list = value as List<object>;
            if (list != null)
            {
                foreach (var item in list)
                {
                    var es = item as string;
                    if (es == null || !schema.EnumValues.Contains(es))
                        ctx.AddError(field, "Invalid enum list value '" + item + "'. Allowed: [" +
                            string.Join(", ", schema.EnumValues) + "]");
                }
                return;
            }

            ctx.AddError(field, "Enum fields must receive a string or list of strings.");
        }

        private void ValidateList(string field, object value, IFieldSchema schema, ValidationContext ctx)
        {
            var list = value as List<object>;
            if (list == null)
            {
                ctx.AddError(field, "Field '" + field + "' expects a list.");
                return;
            }

            foreach (var item in list)
            {
                object coerced;
                if (!TryCoerce(item, schema.ElementType, out coerced))
                    ctx.AddError(field, "List element '" + item + "' cannot be coerced to '" + schema.ElementType + "'.");
            }
        }

        private void ValidateScalar(string field, object value, IFieldSchema schema, ValidationContext ctx)
        {
            object coerced;
            if (!TryCoerce(value, schema.Type, out coerced))
                ctx.AddError(field, "Value '" + value + "' cannot be coerced to '" + schema.Type + "'.");
        }

        // ------------------------------------------------------------
        // COERCION ENGINE
        // ------------------------------------------------------------
        private bool TryCoerce(object input, Type target, out object result)
        {
            result = null;

            if (input == null)
                return true;

            if (target == typeof(string))
            {
                result = input.ToString();
                return true;
            }

            if (target == typeof(int))
            {
                if (input is int) { result = input; return true; }
                var s = input as string;
                int si;
                if (s != null && int.TryParse(s, out si)) { result = si; return true; }
                return false;
            }

            if (target == typeof(float) || target == typeof(double) || target == typeof(decimal))
            {
                double d;
                if (double.TryParse(input.ToString(), out d)) { result = d; return true; }
                return false;
            }

            if (target == typeof(bool))
            {
                if (input is bool) { result = input; return true; }
                var s = input as string;
                bool sb;
                if (s != null && bool.TryParse(s, out sb)) { result = sb; return true; }
                return false;
            }

            if (target == typeof(DateTime))
            {
                if (input is DateTime) { result = input; return true; }
                DateTime parsed;
                if (DateTime.TryParse(input.ToString(), out parsed)) { result = parsed; return true; }
                return false;
            }

            return false;
        }

        // ------------------------------------------------------------
        // VALUE EXTRACTION
        // ------------------------------------------------------------
        private object ExtractValue(ValueNode node, ValidationContext ctx, string path)
        {
            var intNode = node as IntValueNode;
            if (intNode != null) return intNode.Value;

            var floatNode = node as FloatValueNode;
            if (floatNode != null) return floatNode.Value;

            var stringNode = node as StringValueNode;
            if (stringNode != null) return stringNode.Value;

            var boolNode = node as BooleanValueNode;
            if (boolNode != null) return boolNode.Value;

            var enumNode = node as EnumValueNode;
            if (enumNode != null) return enumNode.Value;

            if (node is NullValueNode) return null;

            var listNode = node as ListValueNode;
            if (listNode != null)
            {
                var list = new List<object>();
                foreach (var v in listNode.Values)
                    list.Add(ExtractValue(v, ctx, path));
                return list;
            }

            var objNode = node as ObjectValueNode;
            if (objNode != null)
                return NormalizeInternal(objNode, ctx);

            ctx.AddError(path, "Unsupported value node type '" + node.GetType().Name + "'.");
            return null;
        }

        // ------------------------------------------------------------
        // VALIDATION CONTEXT + SCHEMA-DRIVEN CONSTRAINTS
        // ------------------------------------------------------------
        private sealed class ValidationContext
        {
            private readonly List<ValidationError> _errors = new List<ValidationError>();

            // rangeGroup -> role -> (field, value)
            private readonly Dictionary<string, Dictionary<string, Tuple<string, object>>> _ranges =
                new Dictionary<string, Dictionary<string, Tuple<string, object>>>(StringComparer.OrdinalIgnoreCase);

            // temporalGroup -> role -> (field, value)
            private readonly Dictionary<string, Dictionary<string, Tuple<string, object>>> _temporals =
                new Dictionary<string, Dictionary<string, Tuple<string, object>>>(StringComparer.OrdinalIgnoreCase);

            // geoGroup -> role -> (field, value)
            private readonly Dictionary<string, Dictionary<string, Tuple<string, object>>> _geos =
                new Dictionary<string, Dictionary<string, Tuple<string, object>>>(StringComparer.OrdinalIgnoreCase);

            // fieldName -> (op, value, schema)
            private readonly Dictionary<string, Tuple<string, object, IFieldSchema>> _fieldValues =
                new Dictionary<string, Tuple<string, object, IFieldSchema>>(StringComparer.OrdinalIgnoreCase);

            public string RootType { get; private set; }

            public ValidationContext(string rootType)
            {
                RootType = rootType;
            }

            public void AddError(string path, string message)
            {
                _errors.Add(new ValidationError(path, message));
            }

            public void RegisterRangeBoundary(string group, string role, string field, string op, object value)
            {
                Dictionary<string, Tuple<string, object>> roles;
                if (!_ranges.TryGetValue(group, out roles))
                {
                    roles = new Dictionary<string, Tuple<string, object>>(StringComparer.OrdinalIgnoreCase);
                    _ranges[group] = roles;
                }

                roles[role] = Tuple.Create(field, value);
            }

            public void RegisterTemporal(string group, string role, string field, string op, object value)
            {
                Dictionary<string, Tuple<string, object>> roles;
                if (!_temporals.TryGetValue(group, out roles))
                {
                    roles = new Dictionary<string, Tuple<string, object>>(StringComparer.OrdinalIgnoreCase);
                    _temporals[group] = roles;
                }

                roles[role] = Tuple.Create(field, value);
            }

            public void RegisterGeo(string group, string role, string field, string op, object value)
            {
                Dictionary<string, Tuple<string, object>> roles;
                if (!_geos.TryGetValue(group, out roles))
                {
                    roles = new Dictionary<string, Tuple<string, object>>(StringComparer.OrdinalIgnoreCase);
                    _geos[group] = roles;
                }

                roles[role] = Tuple.Create(field, value);
            }

            public void RegisterFieldValue(string field, string op, object value, IFieldSchema schema)
            {
                _fieldValues[field] = Tuple.Create(op, value, schema);
            }

            public void ThrowIfErrors()
            {
                ValidateRanges();
                ValidateTemporals();
                ValidateGeos();
                ValidateDependencies();

                if (_errors.Count == 0)
                    return;

                var lines = new List<string>();
                lines.Add("{");
                lines.Add("  \"errors\": [");

                for (int i = 0; i < _errors.Count; i++)
                {
                    var e = _errors[i];
                    var comma = (i < _errors.Count - 1) ? "," : "";
                    lines.Add(e.ToJsonObject() + comma);
                }

                lines.Add("  ]");
                lines.Add("}");

                throw new InvalidOperationException(string.Join("\n", lines));
            }

            private void ValidateRanges()
            {
                foreach (var kv in _ranges)
                {
                    var group = kv.Key;
                    var roles = kv.Value;

                    Tuple<string, object> min;
                    Tuple<string, object> max;

                    roles.TryGetValue("min", out min);
                    roles.TryGetValue("max", out max);

                    if (min != null && max != null &&
                        min.Item2 != null && max.Item2 != null)
                    {
                        double minVal = Convert.ToDouble(min.Item2);
                        double maxVal = Convert.ToDouble(max.Item2);

                        if (minVal > maxVal)
                            AddError(min.Item1, "Range group '" + group + "' invalid: min " + minVal + " > max " + maxVal);
                    }
                }
            }

            private void ValidateTemporals()
            {
                foreach (var kv in _temporals)
                {
                    var group = kv.Key;
                    var roles = kv.Value;

                    Tuple<string, object> start;
                    Tuple<string, object> end;

                    roles.TryGetValue("start", out start);
                    roles.TryGetValue("end", out end);

                    if (start != null && end != null &&
                        start.Item2 != null && end.Item2 != null)
                    {
                        DateTime s = Convert.ToDateTime(start.Item2);
                        DateTime e = Convert.ToDateTime(end.Item2);

                        if (s > e)
                            AddError(start.Item1, "Temporal group '" + group + "' invalid: start " + s + " > end " + e);
                    }
                }
            }

            private void ValidateGeos()
            {
                foreach (var kv in _geos)
                {
                    var group = kv.Key;
                    var roles = kv.Value;

                    Tuple<string, object> lat;
                    Tuple<string, object> lng;

                    roles.TryGetValue("lat", out lat);
                    roles.TryGetValue("lng", out lng);

                    if (lat != null && lat.Item2 != null)
                    {
                        double v = Convert.ToDouble(lat.Item2);
                        if (v < -90.0 || v > 90.0)
                            AddError(lat.Item1, "Geo group '" + group + "': latitude out of range: " + v + " (expected -90 to 90).");
                    }

                    if (lng != null && lng.Item2 != null)
                    {
                        double v = Convert.ToDouble(lng.Item2);
                        if (v < -180.0 || v > 180.0)
                            AddError(lng.Item1, "Geo group '" + group + "': longitude out of range: " + v + " (expected -180 to 180).");
                    }
                }
            }

            private void ValidateDependencies()
            {
                foreach (var kv in _fieldValues)
                {
                    var fieldName = kv.Key;
                    var op = kv.Value.Item1;
                    var value = kv.Value.Item2;
                    var schema = kv.Value.Item3;

                    if (schema.Dependencies == null)
                        continue;

                    foreach (var dep in schema.Dependencies)
                    {
                        Tuple<string, object, IFieldSchema> whenFieldTuple;
                        if (!_fieldValues.TryGetValue(dep.WhenField, out whenFieldTuple))
                            continue;

                        var whenValue = whenFieldTuple.Item2;
                        if (whenValue == null)
                            continue;

                        if (!string.Equals(whenValue.ToString(), dep.WhenEquals, StringComparison.OrdinalIgnoreCase))
                            continue;

                        Tuple<string, object, IFieldSchema> targetTuple;
                        if (!_fieldValues.TryGetValue(dep.TargetField, out targetTuple) ||
                            targetTuple.Item2 == null)
                        {
                            AddError(dep.TargetField,
                                "Field '" + dep.TargetField + "' is required when '" +
                                dep.WhenField + "' = '" + dep.WhenEquals + "'.");
                        }
                    }
                }
            }
        }

        private sealed class ValidationError
        {
            public string Path { get; private set; }
            public string Message { get; private set; }

            public ValidationError(string path, string message)
            {
                Path = path;
                Message = message;
            }

            public string ToJsonObject()
            {
                return "    { \"path\": \"" + Path + "\", \"message\": \"" + Message + "\" }";
            }
        }
    }
}