using Cirilla.ViewModels;
using System;
using System.IO;
using System.Text;

namespace Cirilla
{
    public static class Utils
    {
        // Not the best option/naming, but it works I guess
        public static FileTypeTabItemViewModelBase GetViewModelForFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bytes = br.ReadBytes(3);
                string magic = Encoding.ASCII.GetString(bytes);

                switch (magic)
                {
                    case "GMD": return new GMDViewModel(path);
                }

                throw new Exception("This type of files are currently not supported.");
            }
        }
    }
}
