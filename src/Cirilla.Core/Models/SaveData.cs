using Cirilla.Core.Attributes;
using Cirilla.Core.Crypto;
using Cirilla.Core.Crypto.BlowFishCS;
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
using System.Security.Cryptography;
using System.Text;

namespace Cirilla.Core.Models
{
    public class SaveData : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private const string ENCRYPTION_KEY = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";

        public SaveData_Header Header => _header;
        public SaveSlot[] SaveSlots { get; private set; }

        private BlowFish _blowfish = new BlowFish(ExEncoding.ASCII.GetBytes(ENCRYPTION_KEY));
        private SaveData_Header _header;
        private long[] _sectionOffsets;
        private byte[] _sections;

        public SaveData(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);

            // 0x01 00 00 00 == decrypted, something else means that it's encrypted
            bool isUnencrypted = bytes[0] == 0x01 && bytes[1] == 0x00 && bytes[2] == 0x00 && bytes[3] == 0x00;

            if (!isUnencrypted)
            {
                var c = new TanukiSharpCrypto();
                c.Decrypt(bytes);
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                _header = br.ReadStruct<SaveData_Header>();

                if (_header.Magic[0] != 0x01 || _header.Magic[1] != 0x00 || _header.Magic[2] != 0x00 || _header.Magic[3] != 0x00)
                    throw new Exception("Decryption failed or this isn't a valid SAVEDATA file.");

                if (_header.DataSize == 9438368) throw new Exception("This looks like pre-Iceborne SAVEDATA, which is not supported anymore. Try using an older version of this tool.");
                else if (_header.DataSize != 11284640) throw new Exception("Unexpected DataSize, meaning that this tool can't load this SAVEDATA.");

                _sectionOffsets = new long[4];
                _sectionOffsets[0] = br.ReadInt64();
                _sectionOffsets[1] = br.ReadInt64();
                _sectionOffsets[2] = br.ReadInt64();
                _sectionOffsets[3] = br.ReadInt64();

                // See misc/savedata_ib.bt
                _sections = br.ReadBytes(3149948);

                // Load SaveSlots
                SaveSlots = new SaveSlot[3];
                for (int i = 0; i < 3; i++)
                {
                    SaveSlots[i] = new SaveSlot(this, ms);
                }
            }

            Logger.Info($"Successfully loaded '{path}'");
        }

        public long SteamId
        {
            get => _header.SteamId;
            set => _header.SteamId = value;
        }

        public void Save(string path, bool encrypt = true, bool fixChecksum = true)
        {
            Logger.Info($"Saving to '{path}'");

            File.WriteAllBytes(path, GetBytes(encrypt, fixChecksum));

            Logger.Info($"Successfully saved to '{path}'");
        }

        public byte[] GetBytes(bool encrypt = true, bool fixChecksum = true)
        {
            if (SaveSlots.Length != 3)
                throw new Exception("There should be exactly 3 SaveSlots!");

            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                Logger.Info("Writing bytes...");

                bw.Write(_header.ToBytes());

                bw.Write(_sectionOffsets[0]);
                bw.Write(_sectionOffsets[1]);
                bw.Write(_sectionOffsets[2]);
                bw.Write(_sectionOffsets[3]);

                bw.Write(_sections);

                // Write SaveSlots
                for (int i = 0; i < SaveSlots.Length; i++)
                {
                    bw.Write(SaveSlots[i].Native.ToBytes());
                }

                // Fill with zeroes (default array value)
                bytes = new byte[1_724_356];
                bw.Write(bytes);

                // Copy full stream into array so that we can (encrypt and) return it
                bytes = ms.ToArray();
            }

            if (encrypt)
            {
                IceborneCrypto.EncryptRegion(bytes, 0x70, 0xDA50, 3);
                IceborneCrypto.EncryptRegion(bytes, 0x3010D8, 0x2098C0, 0);
                IceborneCrypto.EncryptRegion(bytes, 0x50AB98, 0x2098C0, 1);
                IceborneCrypto.EncryptRegion(bytes, 0x714658, 0x2098C0, 2);
            }

            if (fixChecksum)
            {
                // Update checksum for the whole save data (same as pre-iceborne)
                byte[] checksum = IceborneCrypto.GenerateHash(bytes);
                Array.Copy(checksum, 0, bytes, 12, 20);
            }

            if (encrypt)
            {
                bytes = IceborneCrypto.SwapBytes(bytes);
                bytes = _blowfish.Encrypt_ECB(bytes);
                bytes = IceborneCrypto.SwapBytes(bytes);
            }

            return bytes;
        }
    }

    public class SaveSlot : ICharacterAppearanceProperties, IPalicoAppearanceProperties, ICloneable
    {
        public SaveData SaveData { get; }

        public SaveData_SaveSlot Native => _native;
        private SaveData_SaveSlot _native;

        public SaveSlot(SaveData saveData, Stream stream)
        {
            SaveData = saveData;

            // Don't close stream
            using BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true);
            _native = br.ReadStruct<SaveData_SaveSlot>();
        }

        public SaveSlot(SaveData saveData, SaveData_SaveSlot native)
        {
            SaveData = saveData;
            _native = native;
        }

        public override bool Equals(object obj)
        {
            if (obj is SaveSlot other)
                return _native.Equals(other._native); // Compare struct data

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _native.GetHashCode();
        }

        public object Clone()
        {
            return new SaveSlot(SaveData, _native); // ValueType = passed by value = new object
        }

        public string HunterName
        {
            // TODO: Trim everything after the \0
            get => Encoding.UTF8.GetString(_native.HunterName).TrimZeroTerminator();

            set
            {
                byte[] bytes = new byte[64];
                byte[] cStr = Encoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Hunter name can't use more than 64 bytes, try using a shorter name!");

                Array.Copy(cStr, bytes, cStr.Length);
                _native.HunterName = bytes;
            }
        }

        public string PalicoName
        {
            // TODO: Trim everything after the \0
            get => Encoding.UTF8.GetString(_native.PalicoName).TrimZeroTerminator();

            set
            {
                // Not sure if it is needed to make the array exactly 64 bytes large
                byte[] bytes = new byte[64];
                byte[] cStr = Encoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Palico name can't use more than 64 bytes, try using a shorter name!");

                Array.Copy(cStr, bytes, cStr.Length);
                _native.PalicoName = bytes;
            }
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

        public int Zenny
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

        public int PlayTime
        {
            get => _native.PlayTime;
            set => _native.PlayTime = value;
        }

        #region Character Appearance

        // Group all Appearance getters/setters
        public ICharacterAppearanceProperties CharacterAppearance => this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        // HACK: Makeup is not enclassed (like in native struct) because I'm lazy
        Color ICharacterAppearanceProperties.Makeup2Color
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.Makeup2.Color);
            set => _native.CharacterAppearance.Makeup2.Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup2PosX
        {
            get => _native.CharacterAppearance.Makeup2.PosX;
            set => _native.CharacterAppearance.Makeup2.PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup2PosY
        {
            get => _native.CharacterAppearance.Makeup2.PosY;
            set => _native.CharacterAppearance.Makeup2.PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeX
        {
            get => _native.CharacterAppearance.Makeup2.SizeX;
            set => _native.CharacterAppearance.Makeup2.SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeY
        {
            get => _native.CharacterAppearance.Makeup2.SizeY;
            set => _native.CharacterAppearance.Makeup2.SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup2Glossy
        {
            get => _native.CharacterAppearance.Makeup2.Glossy;
            set => _native.CharacterAppearance.Makeup2.Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup2Metallic
        {
            get => _native.CharacterAppearance.Makeup2.Metallic;
            set => _native.CharacterAppearance.Makeup2.Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup2Type
        {
            get => _native.CharacterAppearance.Makeup2.Type;
            set => _native.CharacterAppearance.Makeup2.Type = value;
        }

        #endregion

        #region Makeup1

        Color ICharacterAppearanceProperties.Makeup1Color
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.Makeup1.Color);
            set => _native.CharacterAppearance.Makeup1.Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup1PosX
        {
            get => _native.CharacterAppearance.Makeup1.PosX;
            set => _native.CharacterAppearance.Makeup1.PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup1PosY
        {
            get => _native.CharacterAppearance.Makeup1.PosY;
            set => _native.CharacterAppearance.Makeup1.PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeX
        {
            get => _native.CharacterAppearance.Makeup1.SizeX;
            set => _native.CharacterAppearance.Makeup1.SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeY
        {
            get => _native.CharacterAppearance.Makeup1.SizeY;
            set => _native.CharacterAppearance.Makeup1.SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup1Glossy
        {
            get => _native.CharacterAppearance.Makeup1.Glossy;
            set => _native.CharacterAppearance.Makeup1.Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup1Metallic
        {
            get => _native.CharacterAppearance.Makeup1.Metallic;
            set => _native.CharacterAppearance.Makeup1.Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup1Type
        {
            get => _native.CharacterAppearance.Makeup1.Type;
            set => _native.CharacterAppearance.Makeup1.Type = value;
        }

        #endregion

        #region Makeup3

        Color ICharacterAppearanceProperties.Makeup3Color
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.Makeup3.Color);
            set => _native.CharacterAppearance.Makeup3.Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup3PosX
        {
            get => _native.CharacterAppearance.Makeup3.PosX;
            set => _native.CharacterAppearance.Makeup3.PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup3PosY
        {
            get => _native.CharacterAppearance.Makeup3.PosY;
            set => _native.CharacterAppearance.Makeup3.PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup3SizeX
        {
            get => _native.CharacterAppearance.Makeup3.SizeX;
            set => _native.CharacterAppearance.Makeup3.SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup3SizeY
        {
            get => _native.CharacterAppearance.Makeup3.SizeY;
            set => _native.CharacterAppearance.Makeup3.SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup3Glossy
        {
            get => _native.CharacterAppearance.Makeup3.Glossy;
            set => _native.CharacterAppearance.Makeup3.Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup3Metallic
        {
            get => _native.CharacterAppearance.Makeup3.Metallic;
            set => _native.CharacterAppearance.Makeup3.Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup3Type
        {
            get => _native.CharacterAppearance.Makeup3.Type;
            set => _native.CharacterAppearance.Makeup3.Type = value;
        }

        #endregion

        #region Colors 1

        Color ICharacterAppearanceProperties.LeftEyeColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.LeftEyeColor);
            set => _native.CharacterAppearance.LeftEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.RightEyeColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.RightEyeColor);
            set => _native.CharacterAppearance.RightEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.EyebrowColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.EyebrowColor);
            set => _native.CharacterAppearance.EyebrowColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.FacialHairColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.FacialHairColor);
            set => _native.CharacterAppearance.FacialHairColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 1

        byte ICharacterAppearanceProperties.EyeWidth
        {
            get => _native.CharacterAppearance.EyeWidth;
            set => _native.CharacterAppearance.EyeWidth = value;
        }

        byte ICharacterAppearanceProperties.EyeHeight
        {
            get => _native.CharacterAppearance.EyeHeight;
            set => _native.CharacterAppearance.EyeHeight = value;
        }

        byte ICharacterAppearanceProperties.SkinColorX
        {
            get => _native.CharacterAppearance.SkinColorX;
            set => _native.CharacterAppearance.SkinColorX = value;
        }

        byte ICharacterAppearanceProperties.SkinColorY
        {
            get => _native.CharacterAppearance.SkinColorY;
            set => _native.CharacterAppearance.SkinColorY = value;
        }

        byte ICharacterAppearanceProperties.Age
        {
            get => _native.CharacterAppearance.Age;
            set => _native.CharacterAppearance.Age = value;
        }

        byte ICharacterAppearanceProperties.Wrinkles
        {
            get => _native.CharacterAppearance.Wrinkles;
            set => _native.CharacterAppearance.Wrinkles = value;
        }

        byte ICharacterAppearanceProperties.NoseHeight
        {
            get => _native.CharacterAppearance.NoseHeight;
            set => _native.CharacterAppearance.NoseHeight = value;
        }

        byte ICharacterAppearanceProperties.MouthHeight
        {
            get => _native.CharacterAppearance.MouthHeight;
            set => _native.CharacterAppearance.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender ICharacterAppearanceProperties.Gender
        {
            get => _native.CharacterAppearance.Gender;
            set => _native.CharacterAppearance.Gender = value;
        }

        #endregion

        #region Types 2

        byte ICharacterAppearanceProperties.BrowType
        {
            get => _native.CharacterAppearance.BrowType;
            set => _native.CharacterAppearance.BrowType = value;
        }

        byte ICharacterAppearanceProperties.FaceType
        {
            get => _native.CharacterAppearance.FaceType;
            set => _native.CharacterAppearance.FaceType = value;
        }

        byte ICharacterAppearanceProperties.EyeType
        {
            get => _native.CharacterAppearance.EyeType;
            set => _native.CharacterAppearance.EyeType = value;
        }

        byte ICharacterAppearanceProperties.NoseType
        {
            get => _native.CharacterAppearance.NoseType;
            set => _native.CharacterAppearance.NoseType = value;
        }

        byte ICharacterAppearanceProperties.MouthType
        {
            get => _native.CharacterAppearance.MouthType;
            set => _native.CharacterAppearance.MouthType = value;
        }

        byte ICharacterAppearanceProperties.EyebrowType
        {
            get => _native.CharacterAppearance.EyebrowType;
            set => _native.CharacterAppearance.EyebrowType = value;
        }

        EyelashLength ICharacterAppearanceProperties.EyelashLength
        {
            get => _native.CharacterAppearance.EyelashLength;
            set => _native.CharacterAppearance.EyelashLength = value;
        }

        byte ICharacterAppearanceProperties.FacialHairType
        {
            get => _native.CharacterAppearance.FacialHairType;
            set => _native.CharacterAppearance.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color ICharacterAppearanceProperties.HairColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.HairColor);
            set => _native.CharacterAppearance.HairColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.ClothingColor
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.ClothingColor);
            set => _native.CharacterAppearance.ClothingColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 3

        short ICharacterAppearanceProperties.HairType
        {
            get => _native.CharacterAppearance.HairType;
            set => _native.CharacterAppearance.HairType = value;
        }

        byte ICharacterAppearanceProperties.ClothingType
        {
            get => _native.CharacterAppearance.ClothingType;
            set => _native.CharacterAppearance.ClothingType = value;
        }

        byte ICharacterAppearanceProperties.Voice
        {
            get => _native.CharacterAppearance.Voice;
            set => _native.CharacterAppearance.Voice = value;
        }

        int ICharacterAppearanceProperties.Expression
        {
            get => _native.CharacterAppearance.Expression;
            set => _native.CharacterAppearance.Expression = value;
        }

        #endregion

        #endregion

        #region Palico Appearance

        public IPalicoAppearanceProperties PalicoAppearance => (IPalicoAppearanceProperties)this;

        Color IPalicoAppearanceProperties.PatternColor1
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.PatternColor1);
            set => _native.PalicoAppearance.PatternColor1 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.PatternColor2
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.PatternColor2);
            set => _native.PalicoAppearance.PatternColor2 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.PatternColor3
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.PatternColor3);
            set => _native.PalicoAppearance.PatternColor3 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.FurColor
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.FurColor);
            set => _native.PalicoAppearance.FurColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.LeftEyeColor
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.LeftEyeColor);
            set => _native.PalicoAppearance.LeftEyeColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.RightEyeColor
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.RightEyeColor);
            set => _native.PalicoAppearance.RightEyeColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.ClothingColor
        {
            get => Utility.RGBAToColor(_native.PalicoAppearance.ClothingColor);
            set => _native.PalicoAppearance.ClothingColor = value.ToRgbaBytes();
        }

        float IPalicoAppearanceProperties.FurLength
        {
            get => _native.PalicoAppearance.FurLength;
            set => _native.PalicoAppearance.FurLength = value;
        }

        float IPalicoAppearanceProperties.FurThickness
        {
            get => _native.PalicoAppearance.FurThickness;
            set => _native.PalicoAppearance.FurThickness = value;
        }

        byte IPalicoAppearanceProperties.PatternType
        {
            get => _native.PalicoAppearance.PatternType;
            set => _native.PalicoAppearance.PatternType = value;
        }

        byte IPalicoAppearanceProperties.EyeType
        {
            get => _native.PalicoAppearance.EyeType;
            set => _native.PalicoAppearance.EyeType = value;
        }

        byte IPalicoAppearanceProperties.EarType
        {
            get => _native.PalicoAppearance.EarType;
            set => _native.PalicoAppearance.EarType = value;
        }

        byte IPalicoAppearanceProperties.TailType
        {
            get => _native.PalicoAppearance.TailType;
            set => _native.PalicoAppearance.TailType = value;
        }

        PalicoVoiceType IPalicoAppearanceProperties.VoiceType
        {
            get => _native.PalicoAppearance.VoiceType;
            set => _native.PalicoAppearance.VoiceType = value;
        }

        PalicoVoicePitch IPalicoAppearanceProperties.VoicePitch
        {
            get => _native.PalicoAppearance.VoicePitch;
            set => _native.PalicoAppearance.VoicePitch = value;
        }

        #endregion
    }
}
