using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using System;

namespace Cirilla.Core.Enums
{
    public class MHFileType : Enumeration
    {
        private static int incr = 0;

        public readonly Type Handler;
        public readonly byte[] Magic;
        public readonly string[] FileExtensions;

        public static MHFileType GMD = new MHFileType(typeof(GMD), "Text file", magic: new byte[] { 0x47, 0x4D, 0x44, 0x00 });
        public static MHFileType SaveData = new MHFileType(typeof(SaveData), "SaveData", magic: new byte[] { 0x72, 0xC8, 0x62, 0x47 });
        public static MHFileType DecryptedSaveData = new MHFileType(typeof(SaveData), "Decrypted SaveData", magic: new byte[] { 0x01, 0x00, 0x00, 0x00 });
        public static MHFileType CMP = new MHFileType(typeof(CMP), "Character Preset", fileExtensions: new[] { ".cmp" });

        public MHFileType() { }

        public MHFileType(Type handler, string desc, byte[] magic = null, string[] fileExtensions = null) : base(incr++, nameof(handler))
        {
            Handler = handler;
            Magic = magic;
            FileExtensions = fileExtensions;
        }
    }
}
