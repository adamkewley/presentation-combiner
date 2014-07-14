using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationCombiner.Behavior
{
    interface IDroppable
    {
        Type DataType { get; }

        void Drop(object data, int index = -1);
    }
}
