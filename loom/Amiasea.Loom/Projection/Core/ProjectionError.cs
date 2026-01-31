using System;
using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionError
    {
        public string Message { get; private set; }
        public IReadOnlyList<string> Path { get; private set; }

        public ProjectionError(string message, IReadOnlyList<string> path)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            Message = message;
            Path = path ?? new List<string>();
        }
    }
}
