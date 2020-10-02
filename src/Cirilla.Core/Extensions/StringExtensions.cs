using System;

namespace Cirilla.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ParseHexString(this string str)
        {
            int num = str.Length;
            byte[] bytes = new byte[num / 2];
            for (int i = 0; i < num; i += 2)
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            return bytes;
        }

        public static string TrimZeroTerminator(this string str)
        {
            int pos = str.IndexOf('\0');
            if (pos >= 0) return str.Substring(0, pos);
            else return str;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
