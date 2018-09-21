using System.Drawing;

namespace Cirilla.Core.Extensions
{
    public static class ColorExtensions
    {
        public static byte[] ToABGR(this Color color)
        {
            return new []
            {
                color.A,
                color.B,
                color.G,
                color.R,
            };
        }
    }
}