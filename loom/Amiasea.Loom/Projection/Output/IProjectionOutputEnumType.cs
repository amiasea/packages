using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionOutputEnumType : IProjectionOutputType
    {
        IReadOnlyList<string> Values { get; }
    }

}
