using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Schema;
using Amiasea.Loom.Schema.Json;

namespace Amiasea.Loom.EF
{
    public sealed class EFSchemaJsonEmitter
    {
        private readonly IProjectionSchema _schema;

        public EFSchemaJsonEmitter(IProjectionSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");
            _schema = schema;
        }

        public void Write(string path, IEnumerable<string> rootTypeNames)
        {
            var json = new JsonSchema();
            json.Types = new List<JsonType>();
            json.RootTypes = new List<string>(rootTypeNames);

            // We assume you have a way to enumerate all types; if not, you can pass them in.
            // For now, we assume root types + their referenced types are enough.

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var root in rootTypeNames)
            {
                AddTypeRecursive(json, root, visited);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var text = JsonConvert.SerializeObject(json, settings);
            File.WriteAllText(path, text, Encoding.UTF8);
        }

        private void AddTypeRecursive(JsonSchema json, string typeName, HashSet<string> visited)
        {
            if (visited.Contains(typeName))
                return;

            visited.Add(typeName);

            var type = _schema.GetTypeByName(typeName);
            var fields = type.GetFields();

            var jt = new JsonType();
            jt.Name = typeName;
            jt.Fields = new List<JsonField>();

            foreach (var f in fields)
            {
                var jf = new JsonField();
                jf.Name = f.Name;
                jf.ClrType = f.Type.FullName;
                jf.Kind = f.Kind.ToString();
                jf.Nullable = f.IsNullable;
                jf.Operators = f.AllowedOperators != null ? f.AllowedOperators.ToList() : null;

                jf.IsEnum = f.IsEnum;
                jf.EnumValues = f.EnumValues != null ? f.EnumValues.ToList() : null;

                jf.IsList = f.IsList;
                jf.ElementClrType = f.ElementType != null ? f.ElementType.FullName : null;

                jf.IsRangeBoundary = f.IsRangeBoundary;
                jf.RangeGroup = f.RangeGroup;
                jf.RangeRole = f.RangeRole;

                jf.TemporalGroup = f.TemporalGroup;
                jf.TemporalRole = f.TemporalRole;

                jf.GeoGroup = f.GeoGroup;
                jf.GeoRole = f.GeoRole;

                if (f.Dependencies != null && f.Dependencies.Count > 0)
                {
                    jf.Dependencies = new List<JsonDependency>();
                    foreach (var d in f.Dependencies)
                    {
                        jf.Dependencies.Add(new JsonDependency
                        {
                            TargetField = d.TargetField,
                            WhenField = d.WhenField,
                            WhenEquals = d.WhenEquals
                        });
                    }
                }

                jt.Fields.Add(jf);
            }

            json.Types.Add(jt);
        }
    }
}