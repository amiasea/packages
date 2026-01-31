using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionListType : IProjectionInputType
    {
        IProjectionInputType ElementType { get; }
    }
}
