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

        public SaveData(string path, bool isEncrypted = true) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);

            if (isEncrypted)
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
                    throw new Exception("Decryption failed!");

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

    public class SaveSlot : IAppearanceProperties
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

        #region Appearance

        // Group all Appearance getters/setters
        public IAppearanceProperties Appearance => (IAppearanceProperties)this;

        // Oh boy do I miss preprocessor macros here...
        // We could probably do some magic like Fody.PropertyChanged does to check if the value is within range

        #region Makeup2

        Color IAppearanceProperties.Makeup2Color
        {
            get => Utility.ABGRToColor(_native.Appearance.Makeup2Color);
            set => _native.Appearance.Makeup2Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float IAppearanceProperties.Makeup2PosX
        {
            get => _native.Appearance.Makeup2PosX;
            set => _native.Appearance.Makeup2PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float IAppearanceProperties.Makeup2PosY
        {
            get => _native.Appearance.Makeup2PosY;
            set => _native.Appearance.Makeup2PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup2SizeX
        {
            get => _native.Appearance.Makeup2SizeX;
            set => _native.Appearance.Makeup2SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup2SizeY
        {
            get => _native.Appearance.Makeup2SizeY;
            set => _native.Appearance.Makeup2SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float IAppearanceProperties.Makeup2Glossy
        {
            get => _native.Appearance.Makeup2Glossy;
            set => _native.Appearance.Makeup2Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float IAppearanceProperties.Makeup2Metallic
        {
            get => _native.Appearance.Makeup2Metallic;
            set => _native.Appearance.Makeup2Metallic = value;
        }

        int IAppearanceProperties.Makeup2Type
        {
            get => _native.Appearance.Makeup2Type;
            set => _native.Appearance.Makeup2Type = value;
        }

        #endregion

        #region Makeup1

        Color IAppearanceProperties.Makeup1Color
        {
            get => Utility.ABGRToColor(_native.Appearance.Makeup1Color);
            set => _native.Appearance.Makeup1Color = value.ToABGR();
        }

        [Range(-0.2f, 0.2f, "0.2 (left) to -0.2 (right)")]
        float IAppearanceProperties.Makeup1PosX
        {
            get => _native.Appearance.Makeup1PosX;
            set => _native.Appearance.Makeup1PosX = value;
        }

        [Range(-0.06f, 0.4f, "0.4 (top) to -0.06 (bottom)")]
        float IAppearanceProperties.Makeup1PosY
        {
            get => _native.Appearance.Makeup1PosY;
            set => _native.Appearance.Makeup1PosY = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup1SizeX
        {
            get => _native.Appearance.Makeup1SizeX;
            set => _native.Appearance.Makeup1SizeX = value;
        }

        [Range(-0.35f, 1.0f, "-0.35 (wide) to 1.0 (narrow)")]
        float IAppearanceProperties.Makeup1SizeY
        {
            get => _native.Appearance.Makeup1SizeY;
            set => _native.Appearance.Makeup1SizeY = value;
        }

        [Range(0.0f, 1.0f, "0.0 (100%) to 1.0 (0%)")]
        float IAppearanceProperties.Makeup1Glossy
        {
            get => _native.Appearance.Makeup1Glossy;
            set => _native.Appearance.Makeup1Glossy = value;
        }

        [Range(0.0f, 1.0f, "0.0 (0%) to 1.0 (100%)")]
        float IAppearanceProperties.Makeup1Metallic
        {
            get => _native.Appearance.Makeup1Metallic;
            set => _native.Appearance.Makeup1Metallic = value;
        }

        int IAppearanceProperties.Makeup1Type
        {
            get => _native.Appearance.Makeup1Type;
            set => _native.Appearance.Makeup1Type = value;
        }

        #endregion

        #region Colors 1

        Color IAppearanceProperties.LeftEyeColor
        {
            get => Utility.ABGRToColor(_native.Appearance.LeftEyeColor);
            set => _native.Appearance.LeftEyeColor = value.ToABGR();
        }

        Color IAppearanceProperties.RightEyeColor
        {
            get => Utility.ABGRToColor(_native.Appearance.RightEyeColor);
            set => _native.Appearance.RightEyeColor = value.ToABGR();
        }

        Color IAppearanceProperties.EyebrowColor
        {
            get => Utility.ABGRToColor(_native.Appearance.EyebrowColor);
            set => _native.Appearance.EyebrowColor = value.ToABGR();
        }

        Color IAppearanceProperties.FacialHairColor
        {
            get => Utility.ABGRToColor(_native.Appearance.FacialHairColor);
            set => _native.Appearance.FacialHairColor = value.ToABGR();
        }

        #endregion

        #region Types 1

        byte IAppearanceProperties.EyeWidth
        {
            get => _native.Appearance.EyeWidth;
            set => _native.Appearance.EyeWidth = value;
        }

        byte IAppearanceProperties.EyeHeight
        {
            get => _native.Appearance.EyeHeight;
            set => _native.Appearance.EyeHeight = value;
        }

        byte IAppearanceProperties.SkinColorX
        {
            get => _native.Appearance.SkinColorX;
            set => _native.Appearance.SkinColorX = value;
        }

        byte IAppearanceProperties.SkinColorY
        {
            get => _native.Appearance.SkinColorY;
            set => _native.Appearance.SkinColorY = value;
        }

        byte IAppearanceProperties.Age
        {
            get => _native.Appearance.Age;
            set => _native.Appearance.Age = value;
        }

        byte IAppearanceProperties.Wrinkles
        {
            get => _native.Appearance.Wrinkles;
            set => _native.Appearance.Wrinkles = value;
        }

        byte IAppearanceProperties.NoseHeight
        {
            get => _native.Appearance.NoseHeight;
            set => _native.Appearance.NoseHeight = value;
        }

        byte IAppearanceProperties.MouthHeight
        {
            get => _native.Appearance.MouthHeight;
            set => _native.Appearance.MouthHeight = value;
        }

        #endregion

        #region Gender

        Gender IAppearanceProperties.Gender
        {
            get => (Gender)_native.Appearance.Gender;
            set => _native.Appearance.Gender = (int)value;
        }

        #endregion

        #region Types 2

        byte IAppearanceProperties.BrowType
        {
            get => _native.Appearance.BrowType;
            set => _native.Appearance.BrowType = value;
        }

        byte IAppearanceProperties.FaceType
        {
            get => _native.Appearance.FaceType;
            set => _native.Appearance.FaceType = value;
        }

        byte IAppearanceProperties.EyeType
        {
            get => _native.Appearance.EyeType;
            set => _native.Appearance.EyeType = value;
        }

        byte IAppearanceProperties.NoseType
        {
            get => _native.Appearance.NoseType;
            set => _native.Appearance.NoseType = value;
        }

        byte IAppearanceProperties.MouthType
        {
            get => _native.Appearance.MouthType;
            set => _native.Appearance.MouthType = value;
        }

        byte IAppearanceProperties.EyebrowType
        {
            get => _native.Appearance.EyebrowType;
            set => _native.Appearance.EyebrowType = value;
        }

        EyelashLength IAppearanceProperties.EyelashLength
        {
            get => (EyelashLength)_native.Appearance.EyelashLength;
            set => _native.Appearance.EyelashLength = (byte)value;
        }

        byte IAppearanceProperties.FacialHairType
        {
            get => _native.Appearance.FacialHairType;
            set => _native.Appearance.FacialHairType = value;
        }

        #endregion

        #region Colors 2

        Color IAppearanceProperties.HairColor
        {
            get => Utility.ABGRToColor(_native.Appearance.HairColor);
            set => _native.Appearance.HairColor = value.ToABGR();
        }

        Color IAppearanceProperties.ClothingColor
        {
            get => Utility.ABGRToColor(_native.Appearance.ClothingColor);
            set => _native.Appearance.ClothingColor = value.ToABGR();
        }

        #endregion

        #region Types 3

        short IAppearanceProperties.HairType
        {
            get => _native.Appearance.HairType;
            set => _native.Appearance.HairType = value;
        }

        byte IAppearanceProperties.ClothingType
        {
            get => _native.Appearance.ClothingType;
            set => _native.Appearance.ClothingType = value;
        }

        byte IAppearanceProperties.Voice
        {
            get => _native.Appearance.Voice;
            set => _native.Appearance.Voice = value;
        }

        int IAppearanceProperties.Expression
        {
            get => _native.Appearance.Expression;
            set => _native.Appearance.Expression = value;
        }

        #endregion

        #endregion
    }
}
