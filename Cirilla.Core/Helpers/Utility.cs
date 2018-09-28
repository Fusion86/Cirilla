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
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        public static Color ABGRToColor(byte[] bytes)
        {
            return Color.FromArgb(
                bytes[0],
                bytes[3],
                bytes[2],
                bytes[1]
            );
        }
    }
}
