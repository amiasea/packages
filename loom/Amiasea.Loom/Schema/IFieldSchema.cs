namespace Amiasea.Loom.Schema
{
    public interface IFieldSchema
    {
        string Name { get; }
        System.Type Type { get; }
        System.Type ElementType { get; }

        FieldKind Kind { get; }
        bool IsNullable { get; }

        bool IsEnum { get; }
        System.Collections.Generic.IReadOnlyList<string> EnumValues { get; }

        bool IsList { get; }
        System.Collections.Generic.IReadOnlyList<string> AllowedOperators { get; }

        // RANGE
        bool IsRangeBoundary { get; }
        string RangeGroup { get; }
        string RangeRole { get; }

        // TEMPORAL
        string TemporalGroup { get; }
        string TemporalRole { get; }

        // GEO
        string GeoGroup { get; }
        string GeoRole { get; }

        // DEPENDENCIES
        System.Collections.Generic.IReadOnlyList<FieldDependency> Dependencies { get; }
    }
}