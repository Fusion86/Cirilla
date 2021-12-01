using Cirilla.Core.Crypto.BlowFishCS;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Interfaces;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cirilla.Core.Models
{
    public class BaseGameSaveData : FileTypeBase, ISaveData
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private const string ENCRYPTION_KEY = "xieZjoe#P2134-3zmaghgpqoe0z8$3azeq";

        public BaseGameSaveData_Header Header => _header;
        public List<BaseGameSaveSlot> SaveSlots { get; private set; }

        private BlowFish _blowfish = new BlowFish(ExEncoding.ASCII.GetBytes(ENCRYPTION_KEY));
        private BaseGameSaveData_Header _header;
        private long[] _sectionOffsets;
        private byte[] _unk1;
        private byte[] _unk4;

        public BaseGameSaveData(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            byte[] bytes = File.ReadAllBytes(path);

            // 0x01 00 00 00 == decrypted, something else means that it's encrypted
            bool isUnencrypted = bytes[0] == 0x01 && bytes[1] == 0x00 && bytes[2] == 0x00 && bytes[3] == 0x00;

            if (!isUnencrypted)
            {
                // BlowFish decryption is rather slow, maybe C would be faster (using P/Invoke)?
                bytes = SwapBytes(bytes);
                bytes = _blowfish.Decrypt_ECB(bytes);
                bytes = SwapBytes(bytes);
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                _header = br.ReadStruct<BaseGameSaveData_Header>();

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
                SaveSlots = new List<BaseGameSaveSlot>(3);

                // Read SaveSlots till the end of the file (max 3 ingame, but there is space for 7)
                //while (ms.Position < ms.Length)
                for (int i = 0; i < 3; i++)
                {
                    SaveSlots.Add(new BaseGameSaveSlot(this, ms));
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

    public class BaseGameSaveSlot : ISaveSlot, ICloneable
    {
        public BaseGameSaveData SaveData { get; }

        public BaseGameSaveData_SaveSlot Native => _native;
        private BaseGameSaveData_SaveSlot _native;

        public BaseGameSaveSlot(BaseGameSaveData saveData, Stream stream)
        {
            SaveData = saveData;

            // Don't close stream
            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                _native = br.ReadStruct<BaseGameSaveData_SaveSlot>();
            }
        }

        public BaseGameSaveSlot(BaseGameSaveData saveData, BaseGameSaveData_SaveSlot native)
        {
            SaveData = saveData;
            _native = native;
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseGameSaveSlot other)
                return _native.Equals(other._native); // Compare struct data

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _native.GetHashCode();
        }

        public object Clone()
        {
            return new BaseGameSaveSlot(SaveData, _native); // ValueType = passed by value = new object
        }

        public string HunterName
        {
            get => Encoding.UTF8.GetString(_native.HunterName).TrimEnd('\0');

            set
            {
                // Not sure if it is needed to make the array exactly 64 bytes large
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
            get => Encoding.UTF8.GetString(_native.PalicoName).TrimEnd('\0');

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
    }
}
