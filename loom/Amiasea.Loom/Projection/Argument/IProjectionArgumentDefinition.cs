using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionArgumentDefinition
    {
        string Name { get; }

        IProjectionInputType Type { get; }

        bool IsNonNull { get; }

        ProjectionArgumentValue DefaultValue { get; }
    }
}