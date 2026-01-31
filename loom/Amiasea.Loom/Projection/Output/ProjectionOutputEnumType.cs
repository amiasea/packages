using System.Collections.Generic;
using System.Linq;
using Amiasea.Loom.Projection;

public sealed class ProjectionOutputEnumType
    : ProjectionOutputTypeBase, IProjectionOutputEnumType
{
    public IReadOnlyList<string> Values { get; }

    public ProjectionOutputEnumType(string name, IEnumerable<string> values)
        : base(name)
    {
        Values = values.ToList();
    }

    public override IProjectionFieldDefinition GetField(string name) => null;
}