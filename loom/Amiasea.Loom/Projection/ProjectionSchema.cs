using System;
using System.Collections.Generic;
using System.Linq;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    // ========================================================================
    //  TYPE SYSTEM CORE
    // ========================================================================

    /// <summary>
    /// Concrete implementation of IProjectionSchema.
    /// Stores all projection types and root types.
    /// </summary>
    public sealed class ProjectionSchema : IProjectionSchema
    {
        private readonly Dictionary<string, IProjectionType> _types;
        private readonly Dictionary<string, IProjectionType> _rootTypes;

        /// <summary>
        /// Creates a new schema from all known types and root types.
        /// </summary>
        public ProjectionSchema(
            IEnumerable<IProjectionType> types,
            IEnumerable<IProjectionType> rootTypes)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (rootTypes == null) throw new ArgumentNullException(nameof(rootTypes));

            _types = types.ToDictionary(t => t.Name, t => t);
            _rootTypes = rootTypes.ToDictionary(t => t.Name, t => t);
        }

        /// <summary>
        /// Gets a root type by name.
        /// </summary>
        public IProjectionType GetRootType(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IProjectionType type;
            if (!_rootTypes.TryGetValue(name, out type))
            {
                throw new InvalidOperationException(
                    "Unknown root type '" + name + "'.");
            }

            return type;
        }

        /// <summary>
        /// Gets any projection type by name.
        /// </summary>
        public IProjectionType GetTypeByName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IProjectionType type;
            if (!_types.TryGetValue(name, out type))
            {
                throw new InvalidOperationException(
                    "Unknown type '" + name + "'.");
            }

            return type;
        }
    }

    // ========================================================================
    //  PROJECTION TYPE BASES
    // ========================================================================

    public abstract class ProjectionTypeBase : IProjectionType
    {
        public string Name { get; private set; }

        protected ProjectionTypeBase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public abstract IProjectionFieldDefinition GetField(string name);
    }

    public abstract class ProjectionOutputTypeBase : ProjectionTypeBase, IProjectionOutputType
    {
        protected ProjectionOutputTypeBase(string name)
            : base(name)
        {
        }
    }

    public abstract class ProjectionInputTypeBase : IProjectionInputType
    {
        public string Name { get; private set; }

        protected ProjectionInputTypeBase(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }

    // ========================================================================
    //  OBJECT TYPE (OUTPUT)
    // ========================================================================

    public sealed class ProjectionObjectType : ProjectionOutputTypeBase
    {
        private readonly Dictionary<string, IProjectionFieldDefinition> _fields;

        public ProjectionObjectType(
            string name,
            IEnumerable<IProjectionFieldDefinition> fields)
            : base(name)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            _fields = fields.ToDictionary(f => f.Name, f => f);
        }

        public override IProjectionFieldDefinition GetField(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IProjectionFieldDefinition field;
            if (!_fields.TryGetValue(name, out field))
            {
                return null;
            }

            return field;
        }
    }

    // ========================================================================
    //  FIELD DEFINITION
    // ========================================================================

    public sealed class ProjectionFieldDefinition : IProjectionFieldDefinition
    {
        public string Name { get; private set; }
        public IProjectionOutputType ReturnType { get; private set; }
        public IReadOnlyList<IProjectionArgumentDefinition> Arguments { get; private set; }
        public bool AllowExtraArguments { get; private set; }

        private readonly Func<string, ProjectionArgumentValue, IProjectionInputType> _extraArgumentTypeResolver;

        public ProjectionFieldDefinition(
            string name,
            IProjectionOutputType returnType,
            IEnumerable<IProjectionArgumentDefinition> arguments,
            bool allowExtraArguments,
            Func<string, ProjectionArgumentValue, IProjectionInputType> extraArgumentTypeResolver)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            Name = name;
            ReturnType = returnType;
            Arguments = arguments.ToArray();
            AllowExtraArguments = allowExtraArguments;
            _extraArgumentTypeResolver = extraArgumentTypeResolver;
        }

        public IProjectionInputType InferExtraArgumentType(
            string name,
            ProjectionArgumentValue rawValue)
        {
            if (!AllowExtraArguments)
            {
                throw new InvalidOperationException(
                    "Field '" + Name + "' does not allow extra arguments.");
            }

            if (_extraArgumentTypeResolver == null)
            {
                throw new InvalidOperationException(
                    "No extra argument type resolver configured for field '" + Name + "'.");
            }

            return _extraArgumentTypeResolver(name, rawValue);
        }
    }

    public sealed class ProjectionArgumentDefinition : IProjectionArgumentDefinition
    {
        public string Name { get; private set; }
        public IProjectionInputType Type { get; private set; }
        public bool IsNonNull { get; private set; }
        public ProjectionArgumentValue DefaultValue { get; private set; }

        public ProjectionArgumentDefinition(
            string name,
            IProjectionInputType type,
            bool isNonNull,
            ProjectionArgumentValue defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            IsNonNull = isNonNull;
            DefaultValue = defaultValue;
        }
    }

    // ========================================================================
    //  SCALAR TYPE
    // ========================================================================

    public sealed class ProjectionScalarType : ProjectionInputTypeBase, IProjectionScalarType
    {
        private readonly Func<object, object> _coercer;

        public ProjectionScalarType(
            string name,
            Func<object, object> coercer)
            : base(name)
        {
            if (coercer == null) throw new ArgumentNullException(nameof(coercer));
            _coercer = coercer;
        }

        public object Coerce(object raw)
        {
            return _coercer(raw);
        }
    }

    // ========================================================================
    //  ENUM TYPE
    // ========================================================================

    public sealed class ProjectionEnumType : ProjectionInputTypeBase, IProjectionEnumType
    {
        private readonly Dictionary<string, object> _values;

        public ProjectionEnumType(
            string name,
            IDictionary<string, object> values)
            : base(name)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            _values = new Dictionary<string, object>(values);
        }

        public object Coerce(string raw)
        {
            if (raw == null) throw new ArgumentNullException(nameof(raw));

            object value;
            if (!_values.TryGetValue(raw, out value))
            {
                throw new InvalidOperationException(
                    "Invalid value '" + raw + "' for enum '" + Name + "'.");
            }

            return value;
        }
    }

    // ========================================================================
    //  LIST TYPE
    // ========================================================================

    public sealed class ProjectionListType : ProjectionInputTypeBase, IProjectionListType
    {
        public IProjectionInputType ElementType { get; private set; }

        public ProjectionListType(IProjectionInputType elementType)
            : base("[" + (elementType == null ? "?" : elementType.Name) + "]")
        {
            if (elementType == null) throw new ArgumentNullException(nameof(elementType));
            ElementType = elementType;
        }
    }

    // ========================================================================
    //  INPUT OBJECT TYPE
    // ========================================================================

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

    public sealed class ProjectionInputFieldDefinition : IProjectionInputFieldDefinition
    {
        public string Name { get; private set; }
        public IProjectionInputType Type { get; private set; }
        public bool IsNonNull { get; private set; }
        public ProjectionArgumentValue DefaultValue { get; private set; }

        public ProjectionInputFieldDefinition(
            string name,
            IProjectionInputType type,
            bool isNonNull,
            ProjectionArgumentValue defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            IsNonNull = isNonNull;
            DefaultValue = defaultValue;
        }
    }
}