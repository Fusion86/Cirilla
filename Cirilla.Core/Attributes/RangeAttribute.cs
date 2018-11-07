using System;

namespace Cirilla.Core.Attributes
{
    public class RangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }
        public string Description { get; }

        public RangeAttribute(float min, float max, string description = null)
        {
            Min = min;
            Max = max;
            Description = description;
        }
    }
}
