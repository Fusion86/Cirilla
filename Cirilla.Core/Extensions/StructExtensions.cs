using Cirilla.Core.Helpers;
using System;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Extensions
{
    public static class StructExtensions
    {
        public static byte[] ToBytes<T>(this T data)
        {
            byte[] bytes = new byte[Marshal.SizeOf(data)];
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }

            EndiannessHelper.RespectEndianness(typeof(T), bytes);

            return bytes;
        }
    }
}
