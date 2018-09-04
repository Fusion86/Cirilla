using Cirilla.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cirilla.Core.Helpers
{
    public static class Utility
    {
        public static MHFileType GetFileType(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bytes = br.ReadBytes(3);
                string magic = Encoding.ASCII.GetString(bytes);

                return Enumeration.GetAll<MHFileType>().FirstOrDefault(x => x.Magics.Contains(magic));
            }
        }

        /// <summary>
        /// Read zero terminated string (skips zeros in front of it)
        /// </summary>
        /// <param name="br"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadZeroTerminatedString(BinaryReader br, Encoding encoding)
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
