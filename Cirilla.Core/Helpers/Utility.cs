using Cirilla.Core.Enums;
using System.IO;
using System.Linq;
using System.Text;

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
                byte[] bytes = br.ReadBytes(3);
                string magic = Encoding.ASCII.GetString(bytes);

                return _fileTypes.Where(x => x.Magics != null).FirstOrDefault(x => x.Magics.Contains(magic));
            }
        }

        private static MHFileType GetFileTypeFromFileExtension(string path)
        {
            string ext = Path.GetExtension(path);
            return _fileTypes.Where(x => x.FileExtensions != null).FirstOrDefault(x => x.FileExtensions.Contains(ext));
        }
    }
}
