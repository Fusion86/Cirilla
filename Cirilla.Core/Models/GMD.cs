using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class GMD
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public GMD_Header Header { get; private set; }
        public string Filename { get; private set; } // size = Header.FilenameLength

        private GMD()
        {

        }

        public static GMD Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                GMD gmd = new GMD();
                gmd.Header = br.ReadStruct<GMD_Header>();

                if (gmd.Header.MagicString != "GMD\0") throw new Exception("Not a GMD file!");

                if (Enum.IsDefined(typeof(EmLanguage), gmd.Header.Language))
                {
                    EmLanguage language = (EmLanguage)gmd.Header.Language;
                    Logger.Info("Language: " + language);
                }
                else
                {
                    Logger.Warn($"Unknown language: 0x{gmd.Header.Language:X04} ({gmd.Header.Language})");
                }

                byte[] bytes = br.ReadBytes((int)gmd.Header.FilenameLength);
                gmd.Filename = Encoding.UTF8.GetString(bytes);

                return gmd;
            }
        }
    }
}
