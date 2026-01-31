using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Amiasea.Loom.Projection
{
    public static class FilterJsonTranslator
    {
        public static string ToJson(FilterNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            using (var stream = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                {
                    Indented = true
                });

                WriteFilter(writer, node);
                writer.Flush();

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static void WriteFilter(Utf8JsonWriter writer, FilterNode node)
        {
            writer.WriteStartObject();

            if (node.And != null && node.And.Count > 0)
            {
                writer.WritePropertyName("and");
                writer.WriteStartArray();
                foreach (var child in node.And)
                {
                    WriteFilter(writer, child);
                }
                writer.WriteEndArray();
            }
            else if (node.Or != null && node.Or.Count > 0)
            {
                writer.WritePropertyName("or");
                writer.WriteStartArray();
                foreach (var child in node.Or)
                {
                    WriteFilter(writer, child);
                }
                writer.WriteEndArray();
            }
            else if (node.Not != null)
            {
                writer.WritePropertyName("not");
                WriteFilter(writer, node.Not);
            }
            else
            {
                if (node.Field == null || node.Operator == null)
                    throw new InvalidOperationException("Invalid comparison FilterNode.");

                writer.WritePropertyName(node.Field);
                writer.WriteStartObject();
                writer.WritePropertyName(node.Operator);
                WriteValue(writer, node.Value);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private static void WriteValue(Utf8JsonWriter writer, object value)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var t = value.GetType();

            if (t == typeof(string))
            {
                writer.WriteStringValue((string)value);
                return;
            }

            if (t == typeof(int))
            {
                writer.WriteNumberValue((int)value);
                return;
            }

            if (t == typeof(long))
            {
                writer.WriteNumberValue((long)value);
                return;
            }

            if (t == typeof(short))
            {
                writer.WriteNumberValue((short)value);
                return;
            }

            if (t == typeof(byte))
            {
                writer.WriteNumberValue((byte)value);
                return;
            }

            if (t == typeof(float))
            {
                writer.WriteNumberValue((float)value);
                return;
            }

            if (t == typeof(double))
            {
                writer.WriteNumberValue((double)value);
                return;
            }

            if (t == typeof(decimal))
            {
                writer.WriteNumberValue((decimal)value);
                return;
            }

            if (t == typeof(bool))
            {
                writer.WriteBooleanValue((bool)value);
                return;
            }

            var enumerable = value as System.Collections.IEnumerable;
            if (enumerable != null && !(value is string))
            {
                writer.WriteStartArray();
                foreach (var item in enumerable)
                {
                    WriteValue(writer, item);
                }
                writer.WriteEndArray();
                return;
            }

            var nestedFilter = value as FilterNode;
            if (nestedFilter != null)
            {
                WriteFilter(writer, nestedFilter);
                return;
            }

            writer.WriteStringValue(value.ToString());
        }

        public static FilterNode FromJson(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                return ParseFilter(root);
            }
        }

        private static FilterNode ParseFilter(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException("Filter JSON must be an object.");

            FilterNode logical;

            if (element.TryGetProperty("and", out var andProp))
            {
                logical = new FilterNode();
                logical.And = new List<FilterNode>();

                foreach (var item in andProp.EnumerateArray())
                {
                    logical.And.Add(ParseFilter(item));
                }

                return logical;
            }

            if (element.TryGetProperty("or", out var orProp))
            {
                logical = new FilterNode();
                logical.Or = new List<FilterNode>();

                foreach (var item in orProp.EnumerateArray())
                {
                    logical.Or.Add(ParseFilter(item));
                }

                return logical;
            }

            if (element.TryGetProperty("not", out var notProp))
            {
                logical = new FilterNode();
                logical.Not = ParseFilter(notProp);
                return logical;
            }

            // Comparison: { field: { op: value } }
            string fieldName = null;
            JsonElement fieldValue = default(JsonElement);

            foreach (var prop in element.EnumerateObject())
            {
                fieldName = prop.Name;
                fieldValue = prop.Value;
                break;
            }

            if (fieldName == null)
                throw new InvalidOperationException("Comparison filter must have exactly one field.");

            if (fieldValue.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException("Comparison operator object must be an object.");

            string opName = null;
            JsonElement opValue = default(JsonElement);

            foreach (var prop in fieldValue.EnumerateObject())
            {
                opName = prop.Name;
                opValue = prop.Value;
                break;
            }

            if (opName == null)
                throw new InvalidOperationException("Comparison operator object must have exactly one operator.");

            var node = new FilterNode();
            node.Field = fieldName;
            node.Operator = opName;
            node.Value = ParseValue(opValue);

            return node;
        }

        private static object ParseValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l))
                        return l;
                    if (element.TryGetDouble(out var d))
                        return d;
                    return element.GetDecimal();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ParseValue(item));
                    }
                    return list;

                case JsonValueKind.Object:
                    // nested filter
                    return ParseFilter(element);

                default:
                    throw new InvalidOperationException("Unsupported JSON value kind: " + element.ValueKind);
            }
        }
    }
}