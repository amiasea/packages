using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Execution
{
    internal sealed class StringScalarType : IProjectionScalarType
    {
        public string Name
        {
            get { return "String"; }
        }

        public object Coerce(object raw)
        {
            return raw == null ? null : raw.ToString();
        }
    }
}
