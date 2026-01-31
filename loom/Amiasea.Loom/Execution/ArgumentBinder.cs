using System;
using System.Collections.Generic;
using Amiasea.Loom.AST;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    public static class ArgumentBinder
    {
        public static IReadOnlyDictionary<string, NormalizedArgumentValue> BindArguments(
            IReadOnlyList<ArgumentNode> astArgs,
            IReadOnlyDictionary<string, IProjectionArgumentDefinition> argDefs)
        {
            var result = new Dictionary<string, NormalizedArgumentValue>(astArgs.Count);

            foreach (var astArg in astArgs)
            {
                IProjectionArgumentDefinition def;
                if (!argDefs.TryGetValue(astArg.Name, out def))
                    throw new InvalidOperationException("Unknown argument '" + astArg.Name + "'.");

                var normalized = BindValue(astArg.Value, def.Type);
                result[astArg.Name] = normalized;
            }

            return result;
        }

        private static NormalizedArgumentValue BindValue(ValueNode node, IProjectionInputType type)
        {
            // NULL
            if (node is NullValueNode)
            {
                return NormalizedArgumentValue.Scalar(null, (IProjectionScalarType)type);
            }

            // INT
            var intNode = node as IntValueNode;
            if (intNode != null)
            {
                return NormalizedArgumentValue.Scalar(intNode.Value, (IProjectionScalarType)type);
            }

            // FLOAT
            var floatNode = node as FloatValueNode;
            if (floatNode != null)
            {
                return NormalizedArgumentValue.Scalar(floatNode.Value, (IProjectionScalarType)type);
            }

            // STRING
            var stringNode = node as StringValueNode;
            if (stringNode != null)
            {
                return NormalizedArgumentValue.Scalar(stringNode.Value, (IProjectionScalarType)type);
            }

            // BOOLEAN
            var boolNode = node as BooleanValueNode;
            if (boolNode != null)
            {
                return NormalizedArgumentValue.Scalar(boolNode.Value, (IProjectionScalarType)type);
            }

            // ENUM
            var enumNode = node as EnumValueNode;
            if (enumNode != null)
            {
                return NormalizedArgumentValue.Enum(enumNode.Value, (IProjectionInputEnumType)type);
            }

            // LIST
            var listNode = node as ListValueNode;
            if (listNode != null)
            {
                var listType = type as ProjectionListInputType;
                if (listType == null)
                    throw new InvalidOperationException("Expected list type for list literal.");

                var elementType = listType.ElementType;
                var items = new List<NormalizedArgumentValue>(listNode.Values.Count);

                foreach (var item in listNode.Values)
                    items.Add(BindValue(item, elementType));

                return NormalizedArgumentValue.List(items, elementType);
            }

            // OBJECT
            var objNode = node as ObjectValueNode;
            if (objNode != null)
            {
                var objType = type as IProjectionInputObjectType;
                if (objType == null)
                    throw new InvalidOperationException("Expected object type for object literal.");

                var fields = new Dictionary<string, NormalizedArgumentValue>();

                foreach (var field in objNode.Fields)
                {
                    IProjectionInputFieldDefinition fieldDef = null;

                    // Try known fields
                    foreach (var f in objType.Fields)
                    {
                        if (f.Name == field.Name)
                        {
                            fieldDef = f;
                            break;
                        }
                    }

                    // Extra fields?
                    if (fieldDef == null)
                    {
                        if (!objType.AllowExtraFields)
                            throw new InvalidOperationException("Unknown input field '" + field.Name + "'.");

                        var inferredType = objType.InferExtraFieldType(field.Name, null);
                        fields[field.Name] = BindValue(field.Value, inferredType);
                        continue;
                    }

                    fields[field.Name] = BindValue(field.Value, fieldDef.Type);
                }

                return NormalizedArgumentValue.Object(fields, objType);
            }

            // UNKNOWN
            throw new InvalidOperationException(
                "Unsupported ValueNode type: " + node.GetType().Name);
        }
    }
}