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
    public class CMP : FileTypeBase, ICharacterAppearanceProperties
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
        public ICharacterAppearanceProperties Appearance => this;

        #region Character Appearance

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        // HACK: Makeup is not enclassed (like in native struct) because I'm lazy
        Color ICharacterAppearanceProperties.Makeup2Color
        {
            get => Utility.RGBAToColor(_native.Makeup2.Color);
            set => _native.Makeup2.Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup2PosX
        {
            get => _native.Makeup2.PosX;
            set => _native.Makeup2.PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup2PosY
        {
            get => _native.Makeup2.PosY;
            set => _native.Makeup2.PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeX
        {
            get => _native.Makeup2.SizeX;
            set => _native.Makeup2.SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeY
        {
            get => _native.Makeup2.SizeY;
            set => _native.Makeup2.SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup2Glossy
        {
            get => _native.Makeup2.Glossy;
            set => _native.Makeup2.Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup2Metallic
        {
            get => _native.Makeup2.Metallic;
            set => _native.Makeup2.Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup2Type
        {
            get => _native.Makeup2.Type;
            set => _native.Makeup2.Type = value;
        }

        #endregion

        #region Makeup1

        Color ICharacterAppearanceProperties.Makeup1Color
        {
            get => Utility.RGBAToColor(_native.Makeup1.Color);
            set => _native.Makeup1.Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup1PosX
        {
            get => _native.Makeup1.PosX;
            set => _native.Makeup1.PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup1PosY
        {
            get => _native.Makeup1.PosY;
            set => _native.Makeup1.PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeX
        {
            get => _native.Makeup1.SizeX;
            set => _native.Makeup1.SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeY
        {
            get => _native.Makeup1.SizeY;
            set => _native.Makeup1.SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup1Glossy
        {
            get => _native.Makeup1.Glossy;
            set => _native.Makeup1.Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup1Metallic
        {
            get => _native.Makeup1.Metallic;
            set => _native.Makeup1.Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup1Type
        {
            get => _native.Makeup1.Type;
            set => _native.Makeup1.Type = value;
        }

        #endregion

        #region Colors 1

        Color ICharacterAppearanceProperties.LeftEyeColor
        {
            get => Utility.RGBAToColor(_native.LeftEyeColor);
            set => _native.LeftEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.RightEyeColor
        {
            get => Utility.RGBAToColor(_native.RightEyeColor);
            set => _native.RightEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.EyebrowColor
        {
            get => Utility.RGBAToColor(_native.EyebrowColor);
            set => _native.EyebrowColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.FacialHairColor
        {
            get => Utility.RGBAToColor(_native.FacialHairColor);
            set => _native.FacialHairColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 1

        byte ICharacterAppearanceProperties.EyeWidth
        {
            get => _native.EyeWidth;
            set => _native.EyeWidth = value;
        }

        byte ICharacterAppearanceProperties.EyeHeight
        {
            get => _native.EyeHeight;
            set => _native.EyeHeight = value;
        }

        byte ICharacterAppearanceProperties.SkinColorX
        {
            get => _native.SkinColorX;
            set => _native.SkinColorX = value;
        }

        byte ICharacterAppearanceProperties.SkinColorY
        {
            get => _native.SkinColorY;
            set => _native.SkinColorY = value;
        }

        byte ICharacterAppearanceProperties.Age
        {
            get => _native.Age;
            set => _native.Age = value;
        }

        byte ICharacterAppearanceProperties.Wrinkles
        {
            get => _native.Wrinkles;
            set => _native.Wrinkles = value;
        }

        byte ICharacterAppearanceProperties.NoseHeight
        {
            get => _native.NoseHeight;
            set => _native.NoseHeight = value;
        }

        byte ICharacterAppearanceProperties.MouthHeight
        {
            get => _native.MouthHeight;
            set => _native.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender ICharacterAppearanceProperties.Gender
        {
            get => _native.Gender;
            set => _native.Gender = value;
        }

        #endregion

        #region Types 2

        byte ICharacterAppearanceProperties.BrowType
        {
            get => _native.BrowType;
            set => _native.BrowType = value;
        }

        byte ICharacterAppearanceProperties.FaceType
        {
            get => _native.FaceType;
            set => _native.FaceType = value;
        }

        byte ICharacterAppearanceProperties.EyeType
        {
            get => _native.EyeType;
            set => _native.EyeType = value;
        }

        byte ICharacterAppearanceProperties.NoseType
        {
            get => _native.NoseType;
            set => _native.NoseType = value;
        }

        byte ICharacterAppearanceProperties.MouthType
        {
            get => _native.MouthType;
            set => _native.MouthType = value;
        }

        byte ICharacterAppearanceProperties.EyebrowType
        {
            get => _native.EyebrowType;
            set => _native.EyebrowType = value;
        }

        EyelashLength ICharacterAppearanceProperties.EyelashLength
        {
            get => _native.EyelashLength;
            set => _native.EyelashLength = value;
        }

        byte ICharacterAppearanceProperties.FacialHairType
        {
            get => _native.FacialHairType;
            set => _native.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color ICharacterAppearanceProperties.HairColor
        {
            get => Utility.RGBAToColor(_native.HairColor);
            set => _native.HairColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.ClothingColor
        {
            get => Utility.RGBAToColor(_native.ClothingColor);
            set => _native.ClothingColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 3

        short ICharacterAppearanceProperties.HairType
        {
            get => _native.HairType;
            set => _native.HairType = value;
        }

        byte ICharacterAppearanceProperties.ClothingType
        {
            get => _native.ClothingType;
            set => _native.ClothingType = value;
        }

        byte ICharacterAppearanceProperties.Voice
        {
            get => _native.Voice;
            set => _native.Voice = value;
        }

        int ICharacterAppearanceProperties.Expression
        {
            get => _native.Expression;
            set => _native.Expression = value;
        }

        #endregion

        #endregion
    }
}
