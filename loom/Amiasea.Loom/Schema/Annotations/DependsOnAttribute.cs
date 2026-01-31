using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Schema.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class DependsOnAttribute : Attribute
    {
        public string WhenField { get; private set; }
        public string WhenEquals { get; private set; }

        public DependsOnAttribute(string whenField, string whenEquals)
        {
            WhenField = whenField;
            WhenEquals = whenEquals;
        }
    }

}
