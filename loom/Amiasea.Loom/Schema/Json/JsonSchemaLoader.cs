using System;
using System.Collections.Generic;
using System.Linq;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Schema;
using Amiasea.Loom.Schema.Json;

namespace Amiasea.Loom.Schema.Json
{
    public static class JsonSchemaLoader
    {
        public static IProjectionSchema Load(JsonSchema json)
        {
            var builder = new SchemaBuilder();

            foreach (var t in json.Types)
            {
                var tb = builder.Type(t.Name);

                foreach (var f in t.Fields)
                {
                    var fb = tb.Field(f.Name);

                    var clrType = Type.GetType(f.ClrType, false) ?? typeof(object);
                    var kind = (FieldKind)Enum.Parse(typeof(FieldKind), f.Kind);

                    fb.Type(clrType, kind)
                      .Nullable(f.Nullable);

                    if (f.IsEnum && f.EnumValues != null)
                        fb.Enum(f.EnumValues.ToArray());

                    if (f.IsList && f.ElementClrType != null)
                    {
                        var elemType = Type.GetType(f.ElementClrType, false) ?? typeof(object);
                        fb.List(elemType);
                    }

                    if (f.Operators != null)
                        fb.Operators(f.Operators.ToArray());

                    if (f.IsRangeBoundary)
                        fb.Range(f.RangeGroup, f.RangeRole);

                    if (f.TemporalGroup != null && f.TemporalRole != null)
                        fb.Temporal(f.TemporalGroup, f.TemporalRole);

                    if (f.GeoGroup != null && f.GeoRole != null)
                        fb.Geo(f.GeoGroup, f.GeoRole);

                    if (f.Dependencies != null)
                    {
                        foreach (var d in f.Dependencies)
                            fb.DependsOn(d.WhenField, d.WhenEquals, d.TargetField);
                    }

                    fb.EndField();
                }

                tb.EndType();
            }

            if (json.RootTypes != null)
            {
                foreach (var r in json.RootTypes)
                    builder.Root(r);
            }

            return builder.Build();
        }
    }
}