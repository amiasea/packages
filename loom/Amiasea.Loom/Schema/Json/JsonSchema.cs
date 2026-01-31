using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Json
{
    public sealed class JsonSchema
    {
        public List<JsonType> Types { get; set; }
        public List<string> RootTypes { get; set; }
    }

}
