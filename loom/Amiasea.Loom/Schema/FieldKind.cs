namespace Amiasea.Loom.Schema
{
    public enum FieldKind
    {
        /// <summary>
        /// Any field type (used for isNull / notNull).
        /// </summary>
        Any = 0,

        /// <summary>
        /// A scalar value: int, float, bool, string (when not treated as string-kind), DateTime.
        /// </summary>
        Scalar = 1,

        /// <summary>
        /// A numeric scalar: int, float, double, decimal.
        /// </summary>
        Numeric = 2,

        /// <summary>
        /// A date or datetime value.
        /// </summary>
        Date = 3,

        /// <summary>
        /// A string field (enables startsWith, endsWith, contains).
        /// </summary>
        String = 4,

        /// <summary>
        /// An enum field with a fixed set of allowed values.
        /// </summary>
        Enum = 5,

        /// <summary>
        /// A list field (list of scalars or enums).
        /// </summary>
        List = 6
    }
}