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
    public class CMP : FileTypeBase, IAppearanceProperties
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

        #region Appearance
        //
        // WARNING
        // THIS WHOLE REGION/SNIPPET IS COPY-PASTED FROM Models/SaveData.cs:215
        // 
        // TODO: DRY I guess?
        //

        // Group all Appearance getters/setters
        public IAppearanceProperties Appearance => (IAppearanceProperties)this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        Color IAppearanceProperties.Makeup2Color
        {
            get => Utility.ABGRToColor(_native.Makeup2Color);
            set => _native.Makeup2Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float IAppearanceProperties.Makeup2PosX
        {
            get => _native.Makeup2PosX;
            set => _native.Makeup2PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float IAppearanceProperties.Makeup2PosY
        {
            get => _native.Makeup2PosY;
            set => _native.Makeup2PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup2SizeX
        {
            get => _native.Makeup2SizeX;
            set => _native.Makeup2SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup2SizeY
        {
            get => _native.Makeup2SizeY;
            set => _native.Makeup2SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float IAppearanceProperties.Makeup2Glossy
        {
            get => _native.Makeup2Glossy;
            set => _native.Makeup2Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float IAppearanceProperties.Makeup2Metallic
        {
            get => _native.Makeup2Metallic;
            set => _native.Makeup2Metallic = value;
        }

        int IAppearanceProperties.Makeup2Type
        {
            get => _native.Makeup2Type;
            set => _native.Makeup2Type = value;
        }

        #endregion

        #region Makeup1

        Color IAppearanceProperties.Makeup1Color
        {
            get => Utility.ABGRToColor(_native.Makeup1Color);
            set => _native.Makeup1Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float IAppearanceProperties.Makeup1PosX
        {
            get => _native.Makeup1PosX;
            set => _native.Makeup1PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float IAppearanceProperties.Makeup1PosY
        {
            get => _native.Makeup1PosY;
            set => _native.Makeup1PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup1SizeX
        {
            get => _native.Makeup1SizeX;
            set => _native.Makeup1SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup1SizeY
        {
            get => _native.Makeup1SizeY;
            set => _native.Makeup1SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float IAppearanceProperties.Makeup1Glossy
        {
            get => _native.Makeup1Glossy;
            set => _native.Makeup1Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float IAppearanceProperties.Makeup1Metallic
        {
            get => _native.Makeup1Metallic;
            set => _native.Makeup1Metallic = value;
        }

        int IAppearanceProperties.Makeup1Type
        {
            get => _native.Makeup1Type;
            set => _native.Makeup1Type = value;
        }

        #endregion

        #region Colors 1

        Color IAppearanceProperties.LeftEyeColor
        {
            get => Utility.ABGRToColor(_native.LeftEyeColor);
            set => _native.LeftEyeColor = value.ToABGR();
        }

        Color IAppearanceProperties.RightEyeColor
        {
            get => Utility.ABGRToColor(_native.RightEyeColor);
            set => _native.RightEyeColor = value.ToABGR();
        }

        Color IAppearanceProperties.EyebrowColor
        {
            get => Utility.ABGRToColor(_native.EyebrowColor);
            set => _native.EyebrowColor = value.ToABGR();
        }

        Color IAppearanceProperties.FacialHairColor
        {
            get => Utility.ABGRToColor(_native.FacialHairColor);
            set => _native.FacialHairColor = value.ToABGR();
        }

        #endregion

        #region Types 1

        byte IAppearanceProperties.EyeWidth
        {
            get => _native.EyeWidth;
            set => _native.EyeWidth = value;
        }

        byte IAppearanceProperties.EyeHeight
        {
            get => _native.EyeHeight;
            set => _native.EyeHeight = value;
        }

        byte IAppearanceProperties.SkinColorX
        {
            get => _native.SkinColorX;
            set => _native.SkinColorX = value;
        }

        byte IAppearanceProperties.SkinColorY
        {
            get => _native.SkinColorY;
            set => _native.SkinColorY = value;
        }

        byte IAppearanceProperties.Age
        {
            get => _native.Age;
            set => _native.Age = value;
        }

        byte IAppearanceProperties.Wrinkles
        {
            get => _native.Wrinkles;
            set => _native.Wrinkles = value;
        }

        byte IAppearanceProperties.NoseHeight
        {
            get => _native.NoseHeight;
            set => _native.NoseHeight = value;
        }

        byte IAppearanceProperties.MouthHeight
        {
            get => _native.MouthHeight;
            set => _native.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender IAppearanceProperties.Gender
        {
            get => (Gender)_native.Gender;
            set => _native.Gender = (int)value;
        }

        #endregion

        #region Types 2

        byte IAppearanceProperties.BrowType
        {
            get => _native.BrowType;
            set => _native.BrowType = value;
        }

        byte IAppearanceProperties.FaceType
        {
            get => _native.FaceType;
            set => _native.FaceType = value;
        }

        byte IAppearanceProperties.EyeType
        {
            get => _native.EyeType;
            set => _native.EyeType = value;
        }

        byte IAppearanceProperties.NoseType
        {
            get => _native.NoseType;
            set => _native.NoseType = value;
        }

        byte IAppearanceProperties.MouthType
        {
            get => _native.MouthType;
            set => _native.MouthType = value;
        }

        byte IAppearanceProperties.EyebrowType
        {
            get => _native.EyebrowType;
            set => _native.EyebrowType = value;
        }

        EyelashLength IAppearanceProperties.EyelashLength
        {
            get => (EyelashLength)_native.EyelashLength;
            set => _native.EyelashLength = (byte)value;
        }

        byte IAppearanceProperties.FacialHairType
        {
            get => _native.FacialHairType;
            set => _native.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color IAppearanceProperties.HairColor
        {
            get => Utility.ABGRToColor(_native.HairColor);
            set => _native.HairColor = value.ToABGR();
        }

        Color IAppearanceProperties.ClothingColor
        {
            get => Utility.ABGRToColor(_native.ClothingColor);
            set => _native.ClothingColor = value.ToABGR();
        }

        #endregion

        #region Types 3

        short IAppearanceProperties.HairType
        {
            get => _native.HairType;
            set => _native.HairType = value;
        }

        byte IAppearanceProperties.ClothingType
        {
            get => _native.ClothingType;
            set => _native.ClothingType = value;
        }

        byte IAppearanceProperties.Voice
        {
            get => _native.Voice;
            set => _native.Voice = value;
        }

        int IAppearanceProperties.Expression
        {
            get => _native.Expression;
            set => _native.Expression = value;
        }

        #endregion

        #endregion
    }
}
