using Cirilla.Core.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Reverse(this byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        public static T ToStruct<T>(this byte[] bytes)
        {
            EndiannessHelper.RespectEndianness(typeof(T), bytes);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
    }
}
