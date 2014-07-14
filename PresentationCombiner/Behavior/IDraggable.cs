using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationCombiner.Behavior
{
    interface IDraggable
    {
        Type DataType { get; }

        void Remove(object item);
    }
}
