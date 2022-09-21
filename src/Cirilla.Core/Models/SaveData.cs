using Cirilla.Core.Crypto;
using Cirilla.Core.Crypto.BlowFishCS;
using Cirilla.Core.Enums;
using Cirilla.Core.Exceptions;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Interfaces;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class SaveData : FileTypeBase, ISaveData
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private const string ENCRYPTION_KEY = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";

        public SaveData_Header Header => _header;
        public SaveSlot[] SaveSlots { get; private set; }

        private readonly BlowFish _blowfish = new BlowFish(ExEncoding.ASCII.GetBytes(ENCRYPTION_KEY));
        private SaveData_Header _header;
        private readonly long[] _sectionOffsets;
        private readonly byte[] _sections;

        public SaveData(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length == 9438432) throw new VanillaSaveGameException("This looks like pre-Iceborne SAVEDATA, which is not supported. Try using an older version of this tool.");

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

                if (_header.DataSize == 9438368) throw new VanillaSaveGameException("This looks like pre-Iceborne SAVEDATA, which is not supported. Try using an older version of this tool.");
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
                    SaveSlots[i] = new SaveSlot(this, ms, i);
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

    public class SaveSlot : ICharacterAppearanceProperties, IPalicoAppearanceProperties, ISaveSlot, ICloneable
    {
        public SaveData SaveData { get; }
        public int SaveSlotIndex { get; }

        public SaveData_SaveSlot Native;

        public SaveSlot(SaveData saveData, Stream stream, int saveSlotIndex)
        {
            SaveData = saveData;
            SaveSlotIndex = saveSlotIndex;

            // Don't close stream
            using BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true);
            Native = br.ReadStruct<SaveData_SaveSlot>();
        }

        public SaveSlot(SaveData saveData, SaveData_SaveSlot native)
        {
            SaveData = saveData;
            Native = native;
        }

        public override bool Equals(object obj)
        {
            if (obj is SaveSlot other)
                return Native.Equals(other.Native); // Compare struct data

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Native.GetHashCode();
        }

        public object Clone()
        {
            return new SaveSlot(SaveData, Native); // ValueType = passed by value = new object
        }

        public string HunterName
        {
            // TODO: Trim everything after the \0
            get => Encoding.UTF8.GetString(Native.HunterName).TrimZeroTerminator();

            set
            {
                byte[] bytes = new byte[64];
                byte[] cStr = Encoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Hunter name can't use more than 64 bytes, try using a shorter name!");

                Array.Copy(cStr, bytes, cStr.Length);
                Native.HunterName = bytes;
            }
        }

        public string PalicoName
        {
            // TODO: Trim everything after the \0
            get => Encoding.UTF8.GetString(Native.PalicoName).TrimZeroTerminator();

            set
            {
                // Not sure if it is needed to make the array exactly 64 bytes large
                byte[] bytes = new byte[64];
                byte[] cStr = Encoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Palico name can't use more than 64 bytes, try using a shorter name!");

                Array.Copy(cStr, bytes, cStr.Length);
                Native.PalicoName = bytes;
            }
        }

        public int HunterRank
        {
            get => Native.HunterRank;
            set
            {
                if (value > 999)
                    throw new Exception("HunterRank can't be higher than 999!");

                Native.HunterRank = value;
            }
        }

        public int MasterRank
        {
            get => Native.MasterRank;
            set
            {
                if (value > 999)
                    throw new Exception("MasterRank can't be higher than 999!");

                Native.MasterRank = value;
            }
        }

        public int Zenny
        {
            get => Native.Zeni;
            set => Native.Zeni = value;
        }

        public int ResearchPoints
        {
            get => Native.ResearchPoints;
            set => Native.ResearchPoints = value;
        }

        public int HunterXp
        {
            get => Native.HunterXp;
            set => Native.HunterXp = value;
        }

        public int MasterXp
        {
            get => Native.MasterXp;
            set => Native.MasterXp = value;
        }

        public int PlayTime
        {
            get => Native.PlayTime;
            set => Native.PlayTime = value;
        }

        // All this trash below here could be rewritten now that we have the `ref` function in C#.
        // That said, I'm not going to touch working code.

        #region Character Appearance

        // Group all Appearance getters/setters
        public ICharacterAppearanceProperties CharacterAppearance => this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        byte[] ICharacterAppearanceProperties.Type
        {
            get => Native.CharacterAppearance.Type;
            set
            {
                Native.CharacterAppearance.Type = value;
            }
        }

        #region Makeup2

        // HACK: Makeup is not enclassed (like in native struct) because I'm lazy
        Color ICharacterAppearanceProperties.Makeup2Color
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.Makeup2.Color);
            set => Native.CharacterAppearance.Makeup2.Color = value.ToRgbaBytes();
        }

        float ICharacterAppearanceProperties.Makeup2PosX
        {
            get => Native.CharacterAppearance.Makeup2.PosX;
            set => Native.CharacterAppearance.Makeup2.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup2PosY
        {
            get => Native.CharacterAppearance.Makeup2.PosY;
            set => Native.CharacterAppearance.Makeup2.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup2SizeX
        {
            get => Native.CharacterAppearance.Makeup2.SizeX;
            set => Native.CharacterAppearance.Makeup2.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup2SizeY
        {
            get => Native.CharacterAppearance.Makeup2.SizeY;
            set => Native.CharacterAppearance.Makeup2.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup2Glossy
        {
            get => Native.CharacterAppearance.Makeup2.Glossy;
            set => Native.CharacterAppearance.Makeup2.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup2Metallic
        {
            get => Native.CharacterAppearance.Makeup2.Metallic;
            set => Native.CharacterAppearance.Makeup2.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup2Luminescent
        {
            get => Native.CharacterAppearance.Makeup2.Luminescent;
            set => Native.CharacterAppearance.Makeup2.Luminescent = value;
        }

        int ICharacterAppearanceProperties.Makeup2Type
        {
            get => Native.CharacterAppearance.Makeup2.Type;
            set => Native.CharacterAppearance.Makeup2.Type = value;
        }

        #endregion

        #region Makeup1

        Color ICharacterAppearanceProperties.Makeup1Color
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.Makeup1.Color);
            set => Native.CharacterAppearance.Makeup1.Color = value.ToRgbaBytes();
        }

        float ICharacterAppearanceProperties.Makeup1PosX
        {
            get => Native.CharacterAppearance.Makeup1.PosX;
            set => Native.CharacterAppearance.Makeup1.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup1PosY
        {
            get => Native.CharacterAppearance.Makeup1.PosY;
            set => Native.CharacterAppearance.Makeup1.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup1SizeX
        {
            get => Native.CharacterAppearance.Makeup1.SizeX;
            set => Native.CharacterAppearance.Makeup1.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup1SizeY
        {
            get => Native.CharacterAppearance.Makeup1.SizeY;
            set => Native.CharacterAppearance.Makeup1.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup1Glossy
        {
            get => Native.CharacterAppearance.Makeup1.Glossy;
            set => Native.CharacterAppearance.Makeup1.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup1Metallic
        {
            get => Native.CharacterAppearance.Makeup1.Metallic;
            set => Native.CharacterAppearance.Makeup1.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup1Luminescent
        {
            get => Native.CharacterAppearance.Makeup1.Luminescent;
            set => Native.CharacterAppearance.Makeup1.Luminescent = value;
        }

        int ICharacterAppearanceProperties.Makeup1Type
        {
            get => Native.CharacterAppearance.Makeup1.Type;
            set => Native.CharacterAppearance.Makeup1.Type = value;
        }

        #endregion

        #region Makeup3

        Color ICharacterAppearanceProperties.Makeup3Color
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.Makeup3.Color);
            set => Native.CharacterAppearance.Makeup3.Color = value.ToRgbaBytes();
        }

        float ICharacterAppearanceProperties.Makeup3PosX
        {
            get => Native.CharacterAppearance.Makeup3.PosX;
            set => Native.CharacterAppearance.Makeup3.PosX = value;
        }

        float ICharacterAppearanceProperties.Makeup3PosY
        {
            get => Native.CharacterAppearance.Makeup3.PosY;
            set => Native.CharacterAppearance.Makeup3.PosY = value;
        }

        float ICharacterAppearanceProperties.Makeup3SizeX
        {
            get => Native.CharacterAppearance.Makeup3.SizeX;
            set => Native.CharacterAppearance.Makeup3.SizeX = value;
        }

        float ICharacterAppearanceProperties.Makeup3SizeY
        {
            get => Native.CharacterAppearance.Makeup3.SizeY;
            set => Native.CharacterAppearance.Makeup3.SizeY = value;
        }

        float ICharacterAppearanceProperties.Makeup3Glossy
        {
            get => Native.CharacterAppearance.Makeup3.Glossy;
            set => Native.CharacterAppearance.Makeup3.Glossy = value;
        }

        float ICharacterAppearanceProperties.Makeup3Metallic
        {
            get => Native.CharacterAppearance.Makeup3.Metallic;
            set => Native.CharacterAppearance.Makeup3.Metallic = value;
        }

        float ICharacterAppearanceProperties.Makeup3Luminescent
        {
            get => Native.CharacterAppearance.Makeup3.Luminescent;
            set => Native.CharacterAppearance.Makeup3.Luminescent = value;
        }

        int ICharacterAppearanceProperties.Makeup3Type
        {
            get => Native.CharacterAppearance.Makeup3.Type;
            set => Native.CharacterAppearance.Makeup3.Type = value;
        }

        #endregion

        #region Colors 1

        Color ICharacterAppearanceProperties.LeftEyeColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.LeftEyeColor);
            set => Native.CharacterAppearance.LeftEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.RightEyeColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.RightEyeColor);
            set => Native.CharacterAppearance.RightEyeColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.EyebrowColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.EyebrowColor);
            set => Native.CharacterAppearance.EyebrowColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.FacialHairColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.FacialHairColor);
            set => Native.CharacterAppearance.FacialHairColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 1

        sbyte ICharacterAppearanceProperties.EyeWidth
        {
            get => Native.CharacterAppearance.EyeWidth;
            set => Native.CharacterAppearance.EyeWidth = value;
        }

        sbyte ICharacterAppearanceProperties.EyeHeight
        {
            get => Native.CharacterAppearance.EyeHeight;
            set => Native.CharacterAppearance.EyeHeight = value;
        }

        byte ICharacterAppearanceProperties.SkinColorX
        {
            get => Native.CharacterAppearance.SkinColorX;
            set => Native.CharacterAppearance.SkinColorX = value;
        }

        byte ICharacterAppearanceProperties.SkinColorY
        {
            get => Native.CharacterAppearance.SkinColorY;
            set => Native.CharacterAppearance.SkinColorY = value;
        }

        byte ICharacterAppearanceProperties.Age
        {
            get => Native.CharacterAppearance.Age;
            set => Native.CharacterAppearance.Age = value;
        }

        byte ICharacterAppearanceProperties.Wrinkles
        {
            get => Native.CharacterAppearance.Wrinkles;
            set => Native.CharacterAppearance.Wrinkles = value;
        }

        sbyte ICharacterAppearanceProperties.NoseHeight
        {
            get => Native.CharacterAppearance.NoseHeight;
            set => Native.CharacterAppearance.NoseHeight = value;
        }

        sbyte ICharacterAppearanceProperties.MouthHeight
        {
            get => Native.CharacterAppearance.MouthHeight;
            set => Native.CharacterAppearance.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender ICharacterAppearanceProperties.Gender
        {
            get => Native.CharacterAppearance.Gender;
            set => Native.CharacterAppearance.Gender = value;
        }

        #endregion

        #region Types 2

        byte ICharacterAppearanceProperties.BrowType
        {
            get => Native.CharacterAppearance.BrowType;
            set => Native.CharacterAppearance.BrowType = value;
        }

        byte ICharacterAppearanceProperties.FaceType
        {
            get => Native.CharacterAppearance.FaceType;
            set => Native.CharacterAppearance.FaceType = value;
        }

        byte ICharacterAppearanceProperties.EyeType
        {
            get => Native.CharacterAppearance.EyeType;
            set => Native.CharacterAppearance.EyeType = value;
        }

        byte ICharacterAppearanceProperties.NoseType
        {
            get => Native.CharacterAppearance.NoseType;
            set => Native.CharacterAppearance.NoseType = value;
        }

        byte ICharacterAppearanceProperties.MouthType
        {
            get => Native.CharacterAppearance.MouthType;
            set => Native.CharacterAppearance.MouthType = value;
        }

        byte ICharacterAppearanceProperties.EyebrowType
        {
            get => Native.CharacterAppearance.EyebrowType;
            set => Native.CharacterAppearance.EyebrowType = value;
        }

        EyelashLength ICharacterAppearanceProperties.EyelashLength
        {
            get => Native.CharacterAppearance.EyelashLength;
            set => Native.CharacterAppearance.EyelashLength = value;
        }

        byte ICharacterAppearanceProperties.FacialHairType
        {
            get => Native.CharacterAppearance.FacialHairType;
            set => Native.CharacterAppearance.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color ICharacterAppearanceProperties.HairColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.HairColor);
            set => Native.CharacterAppearance.HairColor = value.ToRgbaBytes();
        }

        Color ICharacterAppearanceProperties.ClothingColor
        {
            get => Utility.RGBAToColor(Native.CharacterAppearance.ClothingColor);
            set => Native.CharacterAppearance.ClothingColor = value.ToRgbaBytes();
        }

        #endregion

        #region Types 3

        short ICharacterAppearanceProperties.HairType
        {
            get => Native.CharacterAppearance.HairType;
            set => Native.CharacterAppearance.HairType = value;
        }

        byte ICharacterAppearanceProperties.ClothingType
        {
            get => Native.CharacterAppearance.ClothingType;
            set => Native.CharacterAppearance.ClothingType = value;
        }

        byte ICharacterAppearanceProperties.Voice
        {
            get => Native.CharacterAppearance.Voice;
            set => Native.CharacterAppearance.Voice = value;
        }

        int ICharacterAppearanceProperties.Expression
        {
            get => Native.CharacterAppearance.Expression;
            set => Native.CharacterAppearance.Expression = value;
        }

        #endregion

        #endregion

        #region Palico Appearance

        public IPalicoAppearanceProperties PalicoAppearance => (IPalicoAppearanceProperties)this;

        Color IPalicoAppearanceProperties.PatternColor1
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.PatternColor1);
            set => Native.PalicoAppearance.PatternColor1 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.PatternColor2
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.PatternColor2);
            set => Native.PalicoAppearance.PatternColor2 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.PatternColor3
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.PatternColor3);
            set => Native.PalicoAppearance.PatternColor3 = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.FurColor
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.FurColor);
            set => Native.PalicoAppearance.FurColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.LeftEyeColor
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.LeftEyeColor);
            set => Native.PalicoAppearance.LeftEyeColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.RightEyeColor
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.RightEyeColor);
            set => Native.PalicoAppearance.RightEyeColor = value.ToRgbaBytes();
        }

        Color IPalicoAppearanceProperties.ClothingColor
        {
            get => Utility.RGBAToColor(Native.PalicoAppearance.ClothingColor);
            set => Native.PalicoAppearance.ClothingColor = value.ToRgbaBytes();
        }

        float IPalicoAppearanceProperties.FurLength
        {
            get => Native.PalicoAppearance.FurLength;
            set => Native.PalicoAppearance.FurLength = value;
        }

        float IPalicoAppearanceProperties.FurThickness
        {
            get => Native.PalicoAppearance.FurThickness;
            set => Native.PalicoAppearance.FurThickness = value;
        }

        byte IPalicoAppearanceProperties.PatternType
        {
            get => Native.PalicoAppearance.PatternType;
            set => Native.PalicoAppearance.PatternType = value;
        }

        byte IPalicoAppearanceProperties.EyeType
        {
            get => Native.PalicoAppearance.EyeType;
            set => Native.PalicoAppearance.EyeType = value;
        }

        byte IPalicoAppearanceProperties.EarType
        {
            get => Native.PalicoAppearance.EarType;
            set => Native.PalicoAppearance.EarType = value;
        }

        byte IPalicoAppearanceProperties.TailType
        {
            get => Native.PalicoAppearance.TailType;
            set => Native.PalicoAppearance.TailType = value;
        }

        byte IPalicoAppearanceProperties.OutlineType
        {
            get => Native.PalicoAppearance.OutlineType;
            set => Native.PalicoAppearance.OutlineType = value;
        }

        byte IPalicoAppearanceProperties.PupilType
        {
            get => Native.PalicoAppearance.PupilType;
            set => Native.PalicoAppearance.PupilType = value;
        }

        PalicoVoiceType IPalicoAppearanceProperties.VoiceType
        {
            get => Native.PalicoAppearance.VoiceType;
            set => Native.PalicoAppearance.VoiceType = value;
        }

        PalicoVoicePitch IPalicoAppearanceProperties.VoicePitch
        {
            get => Native.PalicoAppearance.VoicePitch;
            set => Native.PalicoAppearance.VoicePitch = value;
        }
        #endregion
    }
}
