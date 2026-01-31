using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Json
{
    public sealed class JsonDependency
    {
        public string TargetField { get; set; }
        public string WhenField { get; set; }
        public string WhenEquals { get; set; }
    }

}
