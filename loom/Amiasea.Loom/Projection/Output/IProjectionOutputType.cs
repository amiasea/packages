using System;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionOutputType
    {
        Type ClrType { get; }
    }
}