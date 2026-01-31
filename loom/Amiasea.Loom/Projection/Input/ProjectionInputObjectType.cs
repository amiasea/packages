using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;

public sealed class ProjectionInputObjectType : ProjectionInputTypeBase, IProjectionInputObjectType
{
    private readonly Dictionary<string, IProjectionInputFieldDefinition> _fields;

    public IReadOnlyList<IProjectionInputFieldDefinition> Fields { get; private set; }
    public bool AllowExtraFields { get; private set; }

    private readonly Func<string, ProjectionArgumentValue, IProjectionInputType> _extraFieldTypeResolver;

    public ProjectionInputObjectType(
        string name,
        IEnumerable<IProjectionInputFieldDefinition> fields,
        bool allowExtraFields,
        Func<string, ProjectionArgumentValue, IProjectionInputType> extraFieldTypeResolver)
        : base(name)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));

        var fieldArray = fields.ToArray();
        Fields = fieldArray;
        _fields = fieldArray.ToDictionary(f => f.Name, f => f);
        AllowExtraFields = allowExtraFields;
        _extraFieldTypeResolver = extraFieldTypeResolver;
    }

    public IProjectionInputType InferExtraFieldType(
        string name,
        ProjectionArgumentValue rawValue)
    {
        if (!AllowExtraFields)
        {
            throw new InvalidOperationException(
                "Input object '" + Name + "' does not allow extra fields.");
        }

        if (_extraFieldTypeResolver == null)
        {
            throw new InvalidOperationException(
                "No extra field type resolver configured for input object '" + Name + "'.");
        }

        return _extraFieldTypeResolver(name, rawValue);
    }
}