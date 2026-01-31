using System;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Schema;

namespace Amiasea.Loom.Tools;

public static class SchemaInspector
{
    public static void Print(IProjectionSchema schema, Action<string> writeLine)
    {
        if (writeLine == null) writeLine = Console.WriteLine;

        // You’ll likely have your own way to enumerate types; here we assume root types only.
        // Extend as needed.

        writeLine("Schema inspection:");

        // This assumes you know the root types externally; otherwise extend IProjectionSchema.
        // For now, this is a placeholder pattern.
        string[] sampleTypes = new string[] { /* fill in or extend schema API */ };

        foreach (var typeName in sampleTypes)
        {
            var type = schema.GetTypeByName(typeName);
            writeLine("Type: " + type.Name);

            foreach (var f in type.GetFields())
            {
                writeLine("  Field: " + f.Name);
                writeLine("    Type: " + f.Type + " (Kind: " + f.Kind + ", Nullable: " + f.IsNullable + ")");
                writeLine("    Operators: " + string.Join(", ", f.AllowedOperators ?? new string[0]));

                if (f.IsEnum)
                    writeLine("    Enum: [" + string.Join(", ", f.EnumValues ?? new string[0]) + "]");

                if (f.IsList)
                    writeLine("    List Element: " + f.ElementType);

                if (f.IsRangeBoundary)
                    writeLine("    Range: Group=" + f.RangeGroup + ", Role=" + f.RangeRole);

                if (f.TemporalGroup != null)
                    writeLine("    Temporal: Group=" + f.TemporalGroup + ", Role=" + f.TemporalRole);

                if (f.GeoGroup != null)
                    writeLine("    Geo: Group=" + f.GeoGroup + ", Role=" + f.GeoRole);

                if (f.Dependencies != null && f.Dependencies.Count > 0)
                {
                    foreach (var d in f.Dependencies)
                    {
                        writeLine("    DependsOn: When " + d.WhenField + " = " + d.WhenEquals +
                                    " => " + d.TargetField + " required");
                    }
                }
            }
        }
    }
}