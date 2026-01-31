using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.AST
{

    // Value nodes – structural only, no schema semantics.
    public abstract class ValueNode { }

    public sealed class NullValueNode : ValueNode
    {
        public static readonly NullValueNode Instance = new NullValueNode();

        private NullValueNode() { }
    }

    public sealed class IntValueNode : ValueNode
    {
        public IntValueNode(long value)
        {
            Value = value;
        }

        public long Value { get; }
    }

    public sealed class FloatValueNode : ValueNode
    {
        public FloatValueNode(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }

    public sealed class StringValueNode : ValueNode
    {
        public StringValueNode(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public sealed class BooleanValueNode : ValueNode
    {
        public BooleanValueNode(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }

    public sealed class EnumValueNode : ValueNode
    {
        public EnumValueNode(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public sealed class ListValueNode : ValueNode
    {
        public ListValueNode(IReadOnlyList<ValueNode> values)
        {
            Values = values;
        }

        public IReadOnlyList<ValueNode> Values { get; }
    }

    public sealed class ObjectValueNode : ValueNode
    {
        public ObjectValueNode(IReadOnlyList<ObjectFieldNode> fields)
        {
            Fields = fields;
        }

        public IReadOnlyList<ObjectFieldNode> Fields { get; }
    }

    public sealed class ObjectFieldNode : ValueNode
    {
        public ObjectFieldNode(string name, ValueNode value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public ValueNode Value { get; }
    }
}