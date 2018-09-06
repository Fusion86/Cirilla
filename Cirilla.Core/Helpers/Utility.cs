using Cirilla.Core.Enums;
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
    }
}
