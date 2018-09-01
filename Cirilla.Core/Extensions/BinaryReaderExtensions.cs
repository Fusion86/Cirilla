using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static T ReadStruct<T>(this BinaryReader br)
        {
#if DEBUG
            // Untested
            if (typeof(T).IsAutoLayout)
                throw new Exception("This might be a problem?");
#endif

            int bytesToRead = Marshal.SizeOf(typeof(T));
            byte[] bytes = br.ReadBytes(bytesToRead);
            return bytes.ToStruct<T>();
        }
    }
}
