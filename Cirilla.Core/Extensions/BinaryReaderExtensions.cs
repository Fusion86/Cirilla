using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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

        /// <summary>
        /// Read zero terminated string (skips zeros in front of it)
        /// </summary>
        /// <param name="br"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadStringZero(this BinaryReader br, Encoding encoding)
        {
            byte b;
            List<byte> szBytes = new List<byte>();

            while (true)
            {
                b = br.ReadByte();

                if (b == 0)
                {
                    // Stop if we found a \0 **AND** we already have read some text.
                    // This is because a string could have empty space in front of it.
                    // While this is 'undocumented behaviour' it works in-game.
                    if (szBytes.Count > 0)
                        break;
                }
                else
                {
                    szBytes.Add(b);
                }
            }

            return encoding.GetString(szBytes.ToArray());
        }
    }
}
