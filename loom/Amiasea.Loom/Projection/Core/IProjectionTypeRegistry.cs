using System;

namespace Amiasea.Loom.Projection
{
    public interface IProjectionTypeRegistry
    {
        /// <summary>
        /// Returns the ProjectionObjectType associated with a CLR type,
        /// or null if the CLR type does not represent an object projection type.
        /// </summary>
        ProjectionObjectType GetObjectType(Type clrType);

        /// <summary>
        /// Returns the IProjectionOutputType (scalar, enum, list, object, non-null)
        /// associated with a CLR type.
        /// </summary>
        IProjectionOutputType GetOutputType(Type clrType);
    }
}