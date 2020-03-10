using System.Text;

namespace Cirilla.Core.Helpers
{
    /// <summary>
    /// Encodings that throw an expcetion when a text can't be encoded or decoded (instead of inserting substitute characters)
    /// </summary>
    public static class ExEncoding
    {
        public static Encoding ASCII = Encoding.GetEncoding("ASCII", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        public static Encoding UTF8 = Encoding.GetEncoding("UTF-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    }
}
