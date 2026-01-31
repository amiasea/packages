using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RangeAttribute : Attribute
    {
        public string Group { get; private set; }
        public string Role { get; private set; }

        public RangeAttribute(string group, string role)
        {
            Group = group;
            Role = role;
        }
    }

}
