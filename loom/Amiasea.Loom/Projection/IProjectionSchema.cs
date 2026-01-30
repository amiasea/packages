using System;
using System.Collections.Generic;
using System.Text;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    // ========================================================================
    //  SCHEMA ABSTRACTIONS
    // ========================================================================

    public interface IProjectionSchema
    {
        IProjectionType GetRootType(string name);

        // ADD THIS
        IProjectionType GetTypeByName(string name);
    }

    public interface IProjectionType
    {
        string Name { get; }

        IProjectionFieldDefinition GetField(string name);
    }

    public interface IProjectionFieldDefinition
    {
        string Name { get; }

        IProjectionOutputType ReturnType { get; }

        IReadOnlyList<IProjectionArgumentDefinition> Arguments { get; }

        bool AllowExtraArguments { get; }

        IProjectionInputType InferExtraArgumentType(
            string name,
            ProjectionArgumentValue rawValue);
    }

    public interface IProjectionArgumentDefinition
    {
        string Name { get; }

        IProjectionInputType Type { get; }

        bool IsNonNull { get; }

        ProjectionArgumentValue DefaultValue { get; }
    }

    public interface IProjectionInputType
    {
        string Name { get; }
    }

    public interface IProjectionOutputType
    {
        string Name { get; }
    }

    public interface IProjectionScalarType : IProjectionInputType
    {
        object Coerce(object raw);
    }

    public interface IProjectionEnumType : IProjectionInputType
    {
        object Coerce(string raw);
    }

    public interface IProjectionListType : IProjectionInputType
    {
        IProjectionInputType ElementType { get; }
    }

    public interface IProjectionInputObjectType : IProjectionInputType
    {
        IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; }

        bool AllowExtraFields { get; }

        IProjectionInputType InferExtraFieldType(
            string name,
            ProjectionArgumentValue rawValue);
    }

    public interface IProjectionInputFieldDefinition
    {
        string Name { get; }

        IProjectionInputType Type { get; }

        bool IsNonNull { get; }

        ProjectionArgumentValue DefaultValue { get; }
    }
}
