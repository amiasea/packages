using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Json
{
    public sealed class JsonType
    {
        public string Name { get; set; }
        public List<JsonField> Fields { get; set; }
    }

}
