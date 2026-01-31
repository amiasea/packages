using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionInputEnumType : IProjectionInputType
    {
        object Coerce(string raw);
    }
}
