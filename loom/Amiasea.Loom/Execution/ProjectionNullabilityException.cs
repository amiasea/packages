using System;

namespace Amiasea.Loom.Execution
{
    public sealed class ProjectionNullabilityException : Exception
    {
        public ProjectionNullabilityException(string message)
            : base(message)
        {
        }

        public ProjectionNullabilityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}