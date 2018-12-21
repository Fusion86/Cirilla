using BlowFishCS;
using Cirilla.Core.Attributes;
using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Interfaces;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cirilla.Core.Models
{
    public class SaveData : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private const string ENCRYPTION_KEY = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";

        public SaveData_Header Header => _header;
        public List<SaveSlot> SaveSlots { get; private set; }

        private BlowFish _blowfish = new BlowFish(ExEncoding.ASCII.GetBytes(ENCRYPTION_KEY));
        private SaveData_Header _header;
        private long[] _sectionOffsets;
        private byte[] _unk1;
        private byte[] _unk4;

        public SaveData(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);

            // 0x01 00 00 00 == decrypted, something else means that it's encrypted
            bool decrypted = bytes[0] == 0x01 && bytes[1] == 0x00 && bytes[2] == 0x00 && bytes[3] == 0x00;          

            if (!decrypted)
            {
                // BlowFish decryption is rather slow, maybe C would be faster (using P/Invoke)?
                bytes = SwapBytes(bytes);
                bytes = _blowfish.Decrypt_ECB(bytes);
                bytes = SwapBytes(bytes);
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                _header = br.ReadStruct<SaveData_Header>();

                if (_header.Magic[0] != 0x01 || _header.Magic[1] != 0x00 || _header.Magic[2] != 0x00 || _header.Magic[3] != 0x00)
                    throw new Exception("Decryption failed or this isn't a valid SAVEDATA1000 file.");

                _sectionOffsets = new long[4];
                _sectionOffsets[0] = br.ReadInt64();
                _sectionOffsets[1] = br.ReadInt64();
                _sectionOffsets[2] = br.ReadInt64();
                _sectionOffsets[3] = br.ReadInt64();

                // There are 3 unk blocks here, to keep things simple we combine it into one block for now
                _unk1 = br.ReadBytes((int)(_sectionOffsets[3] - _sectionOffsets[0]));
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

        public void Save(string path, bool encrypt = true, bool fixChecksum = true)
        {
            Logger.Info($"Saving to '{path}'");

            File.WriteAllBytes(path, GetBytes(encrypt, fixChecksum));
        }

        public byte[] GetBytes(bool encrypt = true, bool fixChecksum = true)
        {
            if (SaveSlots.Count > 3)
                throw new Exception("You can't have more than 3 SaveSlots!");

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

                bw.Write(_unk1);
                bw.Write(_unk4);

                // Write SaveSlots
                //foreach(SaveSlot slot in SaveSlots)
                for (int i = 0; i < 3; i++)
                {
                    bw.Write(SaveSlots[i].Native.ToBytes());
                }

                // Fill with zeroes (default array value)
                bytes = new byte[3_267_796];
                bw.Write(bytes);

                // Copy full stream into array so that we can (encrypt and) return it
                bytes = ms.ToArray();
            }

            if (fixChecksum)
            {
                // Update hash
                // TODO: We can probably do this inside the MemoryStream, without array copying etc
                byte[] checksum = new byte[20];
                SHA1.Create()
                    .ComputeHash(bytes, 64, bytes.Length - 64)
                    .CopyTo(checksum, 0);

                checksum = SwapBytes(checksum);
                Array.Copy(checksum, 0, bytes, 12, 20);
            }

            if (encrypt)
            {
                bytes = SwapBytes(bytes);
                bytes = _blowfish.Encrypt_ECB(bytes);
                bytes = SwapBytes(bytes);
            }

            return bytes;
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

    public class SaveSlot : ICharacterAppearanceProperties, IPalicoAppearanceProperties
    {
        public SaveData_SaveSlot Native => _native;
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

            set
            {
                // Not sure if it is needed to make the array exactly 64 bytes large
                byte[] bytes = new byte[64];
                byte[] cStr = ExEncoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Name is too large!");

                Array.Copy(cStr, bytes, cStr.Length);
                _native.HunterName = bytes;
            }
        }

        public string PalicoName
        {
            get => ExEncoding.UTF8.GetString(_native.PalicoName).TrimEnd('\0');

            set
            {
                // Not sure if it is needed to make the array exactly 64 bytes large
                byte[] bytes = new byte[64];
                byte[] cStr = ExEncoding.UTF8.GetBytes(value);

                if (cStr.Length > 64)
                    throw new Exception("Name is too large!");

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

        #region Character Appearance

        // Group all Appearance getters/setters
        public ICharacterAppearanceProperties CharacterAppearance => (ICharacterAppearanceProperties)this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        Color ICharacterAppearanceProperties.Makeup2Color
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.Makeup2Color);
            set => _native.CharacterAppearance.Makeup2Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup2PosX
        {
            get => _native.CharacterAppearance.Makeup2PosX;
            set => _native.CharacterAppearance.Makeup2PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup2PosY
        {
            get => _native.CharacterAppearance.Makeup2PosY;
            set => _native.CharacterAppearance.Makeup2PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeX
        {
            get => _native.CharacterAppearance.Makeup2SizeX;
            set => _native.CharacterAppearance.Makeup2SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup2SizeY
        {
            get => _native.CharacterAppearance.Makeup2SizeY;
            set => _native.CharacterAppearance.Makeup2SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup2Glossy
        {
            get => _native.CharacterAppearance.Makeup2Glossy;
            set => _native.CharacterAppearance.Makeup2Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup2Metallic
        {
            get => _native.CharacterAppearance.Makeup2Metallic;
            set => _native.CharacterAppearance.Makeup2Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup2Type
        {
            get => _native.CharacterAppearance.Makeup2Type;
            set => _native.CharacterAppearance.Makeup2Type = value;
        }

        #endregion

        #region Makeup1

        Color ICharacterAppearanceProperties.Makeup1Color
        {
            get => Utility.RGBAToColor(_native.CharacterAppearance.Makeup1Color);
            set => _native.CharacterAppearance.Makeup1Color = value.ToRgbaBytes();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float ICharacterAppearanceProperties.Makeup1PosX
        {
            get => _native.CharacterAppearance.Makeup1PosX;
            set => _native.CharacterAppearance.Makeup1PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float ICharacterAppearanceProperties.Makeup1PosY
        {
            get => _native.CharacterAppearance.Makeup1PosY;
            set => _native.CharacterAppearance.Makeup1PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeX
        {
            get => _native.CharacterAppearance.Makeup1SizeX;
            set => _native.CharacterAppearance.Makeup1SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float ICharacterAppearanceProperties.Makeup1SizeY
        {
            get => _native.CharacterAppearance.Makeup1SizeY;
            set => _native.CharacterAppearance.Makeup1SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float ICharacterAppearanceProperties.Makeup1Glossy
        {
            get => _native.CharacterAppearance.Makeup1Glossy;
            set => _native.CharacterAppearance.Makeup1Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float ICharacterAppearanceProperties.Makeup1Metallic
        {
            get => _native.CharacterAppearance.Makeup1Metallic;
            set => _native.CharacterAppearance.Makeup1Metallic = value;
        }

        int ICharacterAppearanceProperties.Makeup1Type
        {
            get => _native.CharacterAppearance.Makeup1Type;
            set => _native.CharacterAppearance.Makeup1Type = value;
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
            get => (Gender)_native.CharacterAppearance.Gender;
            set => _native.CharacterAppearance.Gender = (int)value;
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
            get => (EyelashLength)_native.CharacterAppearance.EyelashLength;
            set => _native.CharacterAppearance.EyelashLength = (byte)value;
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
        
        ushort IPalicoAppearanceProperties.VoiceType
        {
            get => _native.PalicoAppearance.VoiceType;
            set => _native.PalicoAppearance.VoiceType = value;
        }

        [Range(0, 0)]
        ushort IPalicoAppearanceProperties.VoicePitch
        {
            get => _native.PalicoAppearance.VoicePitch;
            set => _native.PalicoAppearance.VoicePitch = value;
        }

        #endregion
    }
}
