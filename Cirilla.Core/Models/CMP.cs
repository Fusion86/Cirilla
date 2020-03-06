using Cirilla.Core.Attributes;
using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Interfaces;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Cirilla.Core.Models
{
    public class CMP : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly static byte[] CMP_MAGIC = new byte[] { 0x07, 0x00 };

        private CharacterAppearance _native;

        public CMP(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] magic = br.ReadBytes(2);

                if (magic.SequenceEqual(CMP_MAGIC) == false)
                    throw new Exception("Not a CMP file!");

                _native = br.ReadStruct<CharacterAppearance>();
            }
        }

        public CMP(CharacterAppearance characterAppearance) : base(null)
        {
            Logger.Info("Creating new CMP based on CharacterAppearance data");
            _native = characterAppearance;
        }

        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(CMP_MAGIC);
                bw.Write(_native.ToBytes());
            }
        }

        //
        // WARNING
        // THIS WHOLE REGION/SNIPPET IS COPY-PASTED FROM Models/SaveData.cs:215
        // 
        // TODO: DRY I guess?
        //

        // Group all Appearance getters/setters
        //public ICharacterAppearanceProperties Appearance => this;

        //Cirilla.Core.Models.CharacterMakeup ICharacterAppearanceProperties.Makeup2;
        //Cirilla.Core.Models.CharacterMakeup ICharacterAppearanceProperties.Makeup1;
    }
}
