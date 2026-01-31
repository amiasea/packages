using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionScalarType : IProjectionInputType
    {
        object Coerce(object raw);
    }
}
