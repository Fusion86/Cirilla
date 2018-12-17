using System.Drawing;

namespace Cirilla.Core.Extensions
{
    public static class ColorExtensions
    {
        public static byte[] ToRgbaBytes(this Color color)
        {
            return new []
            {
                color.R,
                color.G,
                color.B,
                color.A,
            };
        }
    }
}
