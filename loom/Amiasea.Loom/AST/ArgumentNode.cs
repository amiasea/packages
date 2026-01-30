using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.AST
{
    // name: value pair, value is already parsed into a CLR-ish shape
    // (string, int, bool, list, object, etc.)
    public sealed class ArgumentNode
    {
        public string Name { get; }
        public ValueNode Value { get; }

        public ArgumentNode(string name, ValueNode value)
        {
            Name = name;
            Value = value;
        }
    }
}