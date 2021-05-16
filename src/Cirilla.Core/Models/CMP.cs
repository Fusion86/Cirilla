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

        /// <summary>
        /// Magic for base version of the game (pre-Iceborne).
        /// </summary>
        private readonly static byte[] CMP_BASE_MAGIC = new byte[] { 0x07, 0x00 };

        /// <summary>
        /// Magic for Iceborne version of the game.
        /// </summary>
        private readonly static byte[] CMP_ICEBORNE_MAGIC = new byte[] { 0x01, 0x10 };

        private CharacterAppearance _native;

        public CMP(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BinaryReader br = new BinaryReader(fs);

            byte[] magic = br.ReadBytes(2);
            if (magic.SequenceEqual(CMP_BASE_MAGIC))
                _native = br.ReadStruct<Structs.Native.Compat.CharacterAppearance>();
            else if (magic.SequenceEqual(CMP_ICEBORNE_MAGIC))
                _native = br.ReadStruct<CharacterAppearance>();
            else
                throw new Exception("Not a CMP file!");
        }

        public CMP(CharacterAppearance characterAppearance) : base(null)
        {
            Logger.Info("Creating new CMP based on CharacterAppearance data");
            _native = characterAppearance;

            Logger.Info("Updating Type to 'Preset'");
            _native.Type = CharacterAppearanceType.Preset.Value;
        }

        public void Save(string path)
        {
            using FileStream fs = new FileStream(path, FileMode.Create);
            using BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(CMP_ICEBORNE_MAGIC);
            bw.Write(_native.ToBytes());
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

        byte[] ICharacterAppearanceProperties.Type
        {
            get => _native.Type;
            set
            {
                _native.Type = value;
            }
        }

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        // HACK: Makeup is not enclassed (like in native struct) because I'm lazy
        Color ICharacterAppearanceProperties.Makeup2Color
        {
            get => Utility.RGBAToColor(_native.Makeup2.Color);
            set => _native.Makeup2.Color = value.ToRgbaBytes();
        }

        float ICharacterAppearanceProperties.Makeup2PosX
        {
            get => _native.Makeup2.PosX;
            set => _native.Makeup2.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup2PosY
        {
            get => _native.Makeup2.PosY;
            set => _native.Makeup2.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup2SizeX
        {
            get => _native.Makeup2.SizeX;
            set => _native.Makeup2.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup2SizeY
        {
            get => _native.Makeup2.SizeY;
            set => _native.Makeup2.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup2Glossy
        {
            get => _native.Makeup2.Glossy;
            set => _native.Makeup2.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup2Metallic
        {
            get => _native.Makeup2.Metallic;
            set => _native.Makeup2.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup2Luminescent
        {
            get => _native.Makeup2.Luminescent;
            set => _native.Makeup2.Luminescent = value;
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

        float ICharacterAppearanceProperties.Makeup1PosX
        {
            get => _native.Makeup1.PosX;
            set => _native.Makeup1.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup1PosY
        {
            get => _native.Makeup1.PosY;
            set => _native.Makeup1.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup1SizeX
        {
            get => _native.Makeup1.SizeX;
            set => _native.Makeup1.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup1SizeY
        {
            get => _native.Makeup1.SizeY;
            set => _native.Makeup1.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup1Glossy
        {
            get => _native.Makeup1.Glossy;
            set => _native.Makeup1.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup1Metallic
        {
            get => _native.Makeup1.Metallic;
            set => _native.Makeup1.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup1Luminescent
        {
            get => _native.Makeup1.Luminescent;
            set => _native.Makeup1.Luminescent = value;
        }

        int ICharacterAppearanceProperties.Makeup1Type
        {
            get => _native.Makeup1.Type;
            set => _native.Makeup1.Type = value;
        }

        #endregion

        #region Makeup3

        // HACK: Makeup is not enclassed (like in native struct) because I'm lazy
        Color ICharacterAppearanceProperties.Makeup3Color
        {
            get => Utility.RGBAToColor(_native.Makeup3.Color);
            set => _native.Makeup3.Color = value.ToRgbaBytes();
        }

        float ICharacterAppearanceProperties.Makeup3PosX
        {
            get => _native.Makeup3.PosX;
            set => _native.Makeup3.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup3PosY
        {
            get => _native.Makeup3.PosY;
            set => _native.Makeup3.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup3SizeX
        {
            get => _native.Makeup3.SizeX;
            set => _native.Makeup3.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup3SizeY
        {
            get => _native.Makeup3.SizeY;
            set => _native.Makeup3.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup3Glossy
        {
            get => _native.Makeup3.Glossy;
            set => _native.Makeup3.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup3Metallic
        {
            get => _native.Makeup3.Metallic;
            set => _native.Makeup3.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup3Luminescent
        {
            get => _native.Makeup3.Luminescent;
            set => _native.Makeup3.Luminescent = value;
        }

        int ICharacterAppearanceProperties.Makeup3Type
        {
            get => _native.Makeup3.Type;
            set => _native.Makeup3.Type = value;
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

        sbyte ICharacterAppearanceProperties.EyeWidth
        {
            get => _native.EyeWidth;
            set => _native.EyeWidth = value;
        }

        sbyte ICharacterAppearanceProperties.EyeHeight
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

        sbyte ICharacterAppearanceProperties.NoseHeight
        {
            get => _native.NoseHeight;
            set => _native.NoseHeight = value;
        }

        sbyte ICharacterAppearanceProperties.MouthHeight
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
