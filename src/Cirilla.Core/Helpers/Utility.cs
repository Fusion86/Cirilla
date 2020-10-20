using Cirilla.Core.Enums;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Cirilla.Core.Helpers
{
    public static class Utility
    {
        private static readonly MHFileType[] _fileTypes;

        static Utility()
        {
            _fileTypes = _fileTypes = Enumeration.GetAll<MHFileType>().ToArray();
        }

        /// <summary>
        /// Get filetype based on file's magic, and if that doesn't work get filetype based on file extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns>MHFileType or null if not found</returns>
        public static MHFileType GetFileType(string path) => GetFileTypeFromMagic(path) ?? GetFileTypeFromFileExtension(path);

        private static MHFileType GetFileTypeFromMagic(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bytes = br.ReadBytes(4);

                return _fileTypes.Where(x => x.Magic != null).FirstOrDefault(x => x.Magic.SequenceEqual(bytes.Take(x.Magic.Length)));
            }
        }

        private static MHFileType GetFileTypeFromFileExtension(string path)
        {
            string ext = Path.GetExtension(path);
            return _fileTypes.Where(x => x.FileExtensions != null).FirstOrDefault(x => x.FileExtensions.Contains(ext));
        }

        public static Color RGBAToColor(byte[] bytes)
        {
            return Color.FromArgb(
                bytes[3],
                bytes[0],
                bytes[1],
                bytes[2]
            );
        }

        // Taken from https://stackoverflow.com/a/11124118/2125072
        public static string BytesToSizeString(long i)
        {
            // Get absolute value
            long absolute_i = i < 0 ? -i : i;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = i >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = i >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }
}
