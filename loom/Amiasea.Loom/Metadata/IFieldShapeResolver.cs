using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Metadata
{

    public interface IFieldShapeResolver
    {
        FieldShape GetShape(string parentTypeName, string fieldName);
    }
}