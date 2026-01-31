using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Execution
{
    public sealed class OrderByInput
    {
        public string Field { get; private set; }
        public string Direction { get; private set; }

        public OrderByInput(string field, string direction)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (direction == null) throw new ArgumentNullException(nameof(direction));

            Field = field;
            Direction = direction;
        }
    }
}
