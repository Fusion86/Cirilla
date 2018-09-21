using BlowFishCS;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class SaveData : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private const string ENCRYPTION_KEY = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";

        public SaveData_Header Header => _header;
        public List<SaveSlot> SaveSlots;

        private BlowFish _blowfish = new BlowFish(ExEncoding.ASCII.GetBytes(ENCRYPTION_KEY));
        private SaveData_Header _header;
        private long _offset1;
        private long _offset2;
        private long _offset3;
        private long _offset4;
        private byte[] _unk1;
        private byte[] _unk4;

        public SaveData(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);

            // BlowFish decryption is rather slow, maybe C would be faster (using P/Invoke)?
            bytes = SwapBytes(bytes);
            bytes = _blowfish.Decrypt_ECB(bytes);
            bytes = SwapBytes(bytes);

            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                _header = br.ReadStruct<SaveData_Header>();

                if (_header.Magic[0] != 0x01 || _header.Magic[1] != 0x00 || _header.Magic[2] != 0x00 || _header.Magic[3] != 0x00)
                    throw new Exception("Decryption failed!");

                _offset1 = br.ReadInt64();
                _offset2 = br.ReadInt64();
                _offset3 = br.ReadInt64();
                _offset4 = br.ReadInt64();

                // There are 3 unk blocks here, to keep things simple we combine it into one block for now
                _unk1 = br.ReadBytes((int)(_offset4 - _offset1));
                _unk4 = br.ReadBytes(20);

                // Initialize SaveSlots
                SaveSlots = new List<SaveSlot>(3);

                // Read SaveSlots till the end of the file (max 3 ingame, but there is space for 7)
                //while (ms.Position < ms.Length)
                for (int i = 0; i < 3; i++)
                {
                    SaveSlots.Add(new SaveSlot(ms));
                }
            }
        }

        public void Save(string path, bool compressed = true)
        {
            throw new NotImplementedException();
        }

        private static byte[] SwapBytes(byte[] bytes)
        {
            var swapped = new byte[bytes.Length];
            for (var i = 0; i < bytes.Length; i += 4)
            {
                swapped[i] = bytes[i + 3];
                swapped[i + 1] = bytes[i + 2];
                swapped[i + 2] = bytes[i + 1];
                swapped[i + 3] = bytes[i];
            }
            return swapped;
        }
    }

    public class SaveSlot : ISaveSlotAppearanceMethods
    {
        private SaveData_SaveSlot _native;

        public SaveSlot(Stream stream)
        {
            // Don't close stream
            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                _native = br.ReadStruct<SaveData_SaveSlot>();
            }
        }

        public string HunterName
        {
            get => ExEncoding.UTF8.GetString(_native.HunterName).TrimEnd('\0');
            set => throw new NotImplementedException();
        }

        public int HunterRank
        {
            get => _native.HunterRank;
            set
            {
                if (value > 999)
                    throw new Exception("HunterRank can't be higher than 999!");

                _native.HunterRank = value;
            }
        }

        public int Zeni
        {
            get => _native.Zeni;
            set => _native.Zeni = value;
        }

        public int ResearchPoints
        {
            get => _native.ResearchPoints;
            set => _native.ResearchPoints = value;
        }

        public int HunterXp
        {
            get => _native.HunterXp;
            set => _native.HunterXp = value;
        }

        #region Appearance

        // Group all Appearance getters/setters
        public ISaveSlotAppearanceMethods Appearance => (ISaveSlotAppearanceMethods)this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region MakeUp2

        Color ISaveSlotAppearanceMethods.MakeUp2Color
        {
            get => Utility.ABGRToColor(_native.Appearance.MakeUp2Color);
            set => _native.Appearance.MakeUp2Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ISaveSlotAppearanceMethods.MakeUp2PosX
        {
            get => _native.Appearance.MakeUp2PosX;
            set => _native.Appearance.MakeUp2PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ISaveSlotAppearanceMethods.MakeUp2PosY
        {
            get => _native.Appearance.MakeUp2PosY;
            set => _native.Appearance.MakeUp2PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ISaveSlotAppearanceMethods.MakeUp2SizeX
        {
            get => _native.Appearance.MakeUp2SizeX;
            set => _native.Appearance.MakeUp2SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ISaveSlotAppearanceMethods.MakeUp2SizeY
        {
            get => _native.Appearance.MakeUp2SizeY;
            set => _native.Appearance.MakeUp2SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ISaveSlotAppearanceMethods.MakeUp2Glossy
        {
            get => _native.Appearance.MakeUp2Glossy;
            set => _native.Appearance.MakeUp2Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ISaveSlotAppearanceMethods.MakeUp2Metallic
        {
            get => _native.Appearance.MakeUp2Metallic;
            set => _native.Appearance.MakeUp2Metallic = value;
        }

        int ISaveSlotAppearanceMethods.MakeUp2Type
        {
            get => _native.Appearance.MakeUp2Type;
            set => _native.Appearance.MakeUp2Type = value;
        }

        #endregion

        #region MakeUp1

        Color ISaveSlotAppearanceMethods.MakeUp1Color
        {
            get => Utility.ABGRToColor(_native.Appearance.MakeUp1Color);
            set => _native.Appearance.MakeUp1Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ISaveSlotAppearanceMethods.MakeUp1PosX
        {
            get => _native.Appearance.MakeUp1PosX;
            set => _native.Appearance.MakeUp1PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ISaveSlotAppearanceMethods.MakeUp1PosY
        {
            get => _native.Appearance.MakeUp1PosY;
            set => _native.Appearance.MakeUp1PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ISaveSlotAppearanceMethods.MakeUp1SizeX
        {
            get => _native.Appearance.MakeUp1SizeX;
            set => _native.Appearance.MakeUp1SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ISaveSlotAppearanceMethods.MakeUp1SizeY
        {
            get => _native.Appearance.MakeUp1SizeY;
            set => _native.Appearance.MakeUp1SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ISaveSlotAppearanceMethods.MakeUp1Glossy
        {
            get => _native.Appearance.MakeUp1Glossy;
            set => _native.Appearance.MakeUp1Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ISaveSlotAppearanceMethods.MakeUp1Metallic
        {
            get => _native.Appearance.MakeUp1Metallic;
            set => _native.Appearance.MakeUp1Metallic = value;
        }

        int ISaveSlotAppearanceMethods.MakeUp1Type
        {
            get => _native.Appearance.MakeUp1Type;
            set => _native.Appearance.MakeUp1Type = value;
        }

        #endregion

        #region Colors 1

        Color ISaveSlotAppearanceMethods.LeftEyeColor
        {
            get => Utility.ABGRToColor(_native.Appearance.LeftEyeColor);
            set => _native.Appearance.LeftEyeColor = value.ToABGR();
        }

        Color ISaveSlotAppearanceMethods.RightEyeColor
        {
            get => Utility.ABGRToColor(_native.Appearance.RightEyeColor);
            set => _native.Appearance.RightEyeColor = value.ToABGR();
        }

        Color ISaveSlotAppearanceMethods.EyebrowColor
        {
            get => Utility.ABGRToColor(_native.Appearance.EyebrowColor);
            set => _native.Appearance.EyebrowColor = value.ToABGR();
        }

        Color ISaveSlotAppearanceMethods.FacialHairColor
        {
            get => Utility.ABGRToColor(_native.Appearance.FacialHairColor);
            set => _native.Appearance.FacialHairColor = value.ToABGR();
        }

        #endregion

        #region Types 1

        byte ISaveSlotAppearanceMethods.EyeWidth
        {
            get => _native.Appearance.EyeWidth;
            set => _native.Appearance.EyeWidth = value;
        }

        byte ISaveSlotAppearanceMethods.EyeHeight
        {
            get => _native.Appearance.EyeHeight;
            set => _native.Appearance.EyeHeight = value;
        }

        byte ISaveSlotAppearanceMethods.SkinColorX
        {
            get => _native.Appearance.SkinColorX;
            set => _native.Appearance.SkinColorX = value;
        }

        byte ISaveSlotAppearanceMethods.SkinColorY
        {
            get => _native.Appearance.SkinColorY;
            set => _native.Appearance.SkinColorY = value;
        }

        byte ISaveSlotAppearanceMethods.Age
        {
            get => _native.Appearance.Age;
            set => _native.Appearance.Age = value;
        }

        byte ISaveSlotAppearanceMethods.Wrinkles
        {
            get => _native.Appearance.Wrinkles;
            set => _native.Appearance.Wrinkles = value;
        }

        byte ISaveSlotAppearanceMethods.NoseHeight
        {
            get => _native.Appearance.NoseHeight;
            set => _native.Appearance.NoseHeight = value;
        }

        byte ISaveSlotAppearanceMethods.MouthHeight
        {
            get => _native.Appearance.MouthHeight;
            set => _native.Appearance.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender ISaveSlotAppearanceMethods.Gender
        {
            get => (Gender)_native.Appearance.Gender;
            set => _native.Appearance.Gender = (int)value;
        }

        #endregion

        #region Types 2

        byte ISaveSlotAppearanceMethods.BrowType
        {
            get => _native.Appearance.BrowType;
            set => _native.Appearance.BrowType = value;
        }

        byte ISaveSlotAppearanceMethods.FaceType
        {
            get => _native.Appearance.FaceType;
            set => _native.Appearance.FaceType = value;
        }

        byte ISaveSlotAppearanceMethods.EyeType
        {
            get => _native.Appearance.EyeType;
            set => _native.Appearance.EyeType = value;
        }

        byte ISaveSlotAppearanceMethods.NoseType
        {
            get => _native.Appearance.NoseType;
            set => _native.Appearance.NoseType = value;
        }

        byte ISaveSlotAppearanceMethods.MouthType
        {
            get => _native.Appearance.MouthType;
            set => _native.Appearance.MouthType = value;
        }

        byte ISaveSlotAppearanceMethods.EyebrowType
        {
            get => _native.Appearance.EyebrowType;
            set => _native.Appearance.EyebrowType = value;
        }

        EyelashLength ISaveSlotAppearanceMethods.EyelashLength
        {
            get => (EyelashLength)_native.Appearance.EyelashLength;
            set => _native.Appearance.EyelashLength = (byte)value;
        }

        byte ISaveSlotAppearanceMethods.FacialHairType
        {
            get => _native.Appearance.FacialHairType;
            set => _native.Appearance.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color ISaveSlotAppearanceMethods.HairColor
        {
            get => Utility.ABGRToColor(_native.Appearance.HairColor);
            set => _native.Appearance.HairColor = value.ToABGR();
        }

        Color ISaveSlotAppearanceMethods.ClothingColor
        {
            get => Utility.ABGRToColor(_native.Appearance.ClothingColor);
            set => _native.Appearance.ClothingColor = value.ToABGR();
        }

        #endregion

        #region Types 3

        short ISaveSlotAppearanceMethods.HairType
        {
            get => _native.Appearance.HairType;
            set => _native.Appearance.HairType = value;
        }

        byte ISaveSlotAppearanceMethods.ClothingType
        {
            get => _native.Appearance.ClothingType;
            set => _native.Appearance.ClothingType = value;
        }

        byte ISaveSlotAppearanceMethods.Voice
        {
            get => _native.Appearance.Voice;
            set => _native.Appearance.Voice = value;
        }

        int ISaveSlotAppearanceMethods.Expression
        {
            get => _native.Appearance.Expression;
            set => _native.Appearance.Expression = value;
        }

        #endregion

        #endregion
    }

    public interface ISaveSlotAppearanceMethods
    {
        Color MakeUp2Color { get; set; }
        float MakeUp2PosX { get; set; }
        float MakeUp2PosY { get; set; }
        float MakeUp2SizeX { get; set; }
        float MakeUp2SizeY { get; set; }
        float MakeUp2Glossy { get; set; }
        float MakeUp2Metallic { get; set; }
        int MakeUp2Type { get; set; }

        Color MakeUp1Color { get; set; }
        float MakeUp1PosX { get; set; }
        float MakeUp1PosY { get; set; }
        float MakeUp1SizeX { get; set; }
        float MakeUp1SizeY { get; set; }
        float MakeUp1Glossy { get; set; }
        float MakeUp1Metallic { get; set; }
        int MakeUp1Type { get; set; }

        Color LeftEyeColor { get; set; }
        Color RightEyeColor { get; set; }
        Color EyebrowColor { get; set; }
        Color FacialHairColor { get; set; }

        byte EyeWidth { get; set; }
        byte EyeHeight { get; set; }
        byte SkinColorX { get; set; }
        byte SkinColorY { get; set; }
        byte Age { get; set; }
        byte Wrinkles { get; set; }
        byte NoseHeight { get; set; }
        byte MouthHeight { get; set; }

        Gender Gender { get; set; }

        byte BrowType { get; set; }
        byte FaceType { get; set; }
        byte EyeType { get; set; }
        byte NoseType { get; set; }
        byte MouthType { get; set; }
        byte EyebrowType { get; set; }
        EyelashLength EyelashLength { get; set; }
        byte FacialHairType { get; set; }

        Color HairColor { get; set; }
        Color ClothingColor { get; set; }

        short HairType { get; set; }
        byte ClothingType { get; set; }
        byte Voice { get; set; }
        int Expression { get; set; }
    }

    public class RangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }
        public string Description { get; }

        public RangeAttribute(float min, float max, string description = null)
        {
            Min = min;
            Max = max;
            Description = description;
        }
    }

    public enum Gender : int
    {
        Male = 0,
        Female = 1
    }

    public enum EyelashLength : byte
    {
        Short = 0,
        Average = 1,
        Long = 2
    }
}
