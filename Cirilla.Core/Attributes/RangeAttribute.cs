using System;

namespace Cirilla.Core.Attributes
{
    public class RangeAttribute : Attribute
    {
        public object Min { get; }
        public object Max { get; }
        public string Description { get; }

        public RangeAttribute(object min, object max, string description = null)
        {
            Min = min;
            Max = max;
            Description = description;
        }
    }
}
