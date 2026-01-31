using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Json
{
    public sealed class JsonField
    {
        public string Name { get; set; }
        public string ClrType { get; set; }
        public string Kind { get; set; }
        public bool Nullable { get; set; }

        public List<string> Operators { get; set; }

        public bool IsEnum { get; set; }
        public List<string> EnumValues { get; set; }

        public bool IsList { get; set; }
        public string ElementClrType { get; set; }

        public bool IsRangeBoundary { get; set; }
        public string RangeGroup { get; set; }
        public string RangeRole { get; set; }

        public string TemporalGroup { get; set; }
        public string TemporalRole { get; set; }

        public string GeoGroup { get; set; }
        public string GeoRole { get; set; }

        public List<JsonDependency> Dependencies { get; set; }
    }

}
