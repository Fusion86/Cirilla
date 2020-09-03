using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cirilla.Core.Models
{
    public class GMD : FileTypeBase
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Read-Only header.
        /// </summary>
        public GMD_Header Header => _header;

        public string Filename { get; }
        public List<IGMD_Entry> Entries { get; }

        private GMD_Header _header;
        private byte[] _unk1;

        public GMD(string path) : base(path)
        {
            log.Info($"Loading '{path}'");

            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BinaryReader br = new BinaryReader(fs);
            
            // Header
            _header = br.ReadStruct<GMD_Header>();

            if (_header.MagicString != "GMD")
                throw new Exception("Not a GMD file!");

            // Log some info about the GMD language
            if (Enum.IsDefined(typeof(EmLanguage), _header.Language))
            {
                EmLanguage language = (EmLanguage)_header.Language;
                log.Info("Language: " + language);
            }
            else
            {
                log.Warn($"Unknown language: 0x{_header.Language:X04} ({_header.Language})");
            }

            // Filename
            Filename = br.ReadStringZero(ExEncoding.ASCII);

            // Set Entries initial capacity
            Entries = new List<IGMD_Entry>(_header.StringCount);

            // Info Table
            GMD_Entry lastEntry = null;
            for (int i = 0; i < _header.KeyCount; i++)
            {
                GMD_Entry entry = new GMD_Entry { InfoTableEntry = br.ReadStruct<GMD_InfoTableEntry>() };

                int lastStringIndex = lastEntry?.InfoTableEntry.StringIndex ?? 0;
                lastEntry = entry;

                for (int j = lastStringIndex + 1; j < entry.InfoTableEntry.StringIndex; j++)
                    Entries.Add(new GMD_EntryWithoutKey());

                Entries.Add(entry);
            }

            // If there are "Invalid Message" entries after the last valid entry then the above code won't add GMD_EntryWithoutKey's for those entries
            // So here we add GMD_EntryWithoutKey entries till we have {StringCount} amount of them
            for (int i = Entries.Count; i < _header.StringCount; i++)
                Entries.Add(new GMD_EntryWithoutKey());

            // Block with unknown data
            _unk1 = br.ReadBytes(0x800);

            // Keys, this skips over the GMD_EntryWithoutKey entries
            foreach (GMD_Entry entry in Entries.OfType<GMD_Entry>())
                entry.Key = br.ReadStringZero(ExEncoding.UTF8);

            // Strings
            string[] strings = new string[_header.StringCount];
            long startOfStringBlock = fs.Position;

            for (int i = 0; i < _header.StringCount; i++)
            {
                if (fs.Position == fs.Length)
                {
                    log.Warn($"Expected to read {_header.StringCount - i} more strings (for a total of {_header.StringCount}). But we already are at the end of the stream!");
                    break;
                }

                strings[i] = br.ReadStringZero(ExEncoding.UTF8);
            }

            log.Info("Expected StringBlockSize = " + _header.StringBlockSize);
            log.Info("Actual StringBlockSize = " + (fs.Position - startOfStringBlock));

            if (_header.StringBlockSize != (fs.Position - startOfStringBlock))
                log.Warn("Actual StringBlockSize is not the same as the expected StringBlockSize!");

            for (int i = 0; i < _header.StringCount; i++)
                Entries[i].Value = strings[i];
        }

        public void Save(string path)
        {
            log.Info($"Saving {Filename} to '{path}'");

            Update();

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                log.Info("Writing bytes...");

                bw.Write(_header.ToBytes());
                bw.Write(ExEncoding.ASCII.GetBytes(Filename));
                bw.Write((byte)0); // szString end of string

                // Info Table, excludes GMD_EntryWithoutKey
                foreach (GMD_Entry entry in Entries.OfType<GMD_Entry>())
                {
                    bw.Write(entry.InfoTableEntry.ToBytes());
                }

                bw.Write(_unk1);

                // Keys, excludes GMD_EntryWithoutKey
                foreach (GMD_Entry entry in Entries.OfType<GMD_Entry>())
                {
                    bw.Write(ExEncoding.UTF8.GetBytes(entry.Key));
                    bw.Write((byte)0); // szString end of string
                }

                // Strings, __includes__ GMD_EntryWithoutKey
                foreach (IGMD_Entry entry in Entries)
                {
                    bw.Write(ExEncoding.UTF8.GetBytes(entry.Value));
                    bw.Write((byte)0); // szString end of string
                }
            }

            log.Info("Saved file!");
        }

        /// <summary>
        /// Update header
        /// This is needed when anything changes
        /// </summary>
        private void Update()
        {
            log.Info("Updating entries...");

            var realEntries = Entries.OfType<GMD_Entry>().ToList();

            // First info entry always has Index = 0 and KeyOffset = 0
            realEntries[0].InfoTableEntry.StringIndex = 0;
            realEntries[0].InfoTableEntry.KeyOffset = 0;

            for (int i = 1; i < realEntries.Count; i++) // Start at 1
            {
                realEntries[i].InfoTableEntry.StringIndex = Entries.IndexOf(realEntries[i]);
                int prevKeyOffset = realEntries[i - 1].InfoTableEntry.KeyOffset;
                int prevKeySize = ExEncoding.UTF8.GetByteCount(realEntries[i - 1].Key) + 1; // +1 for szString end
                realEntries[i].InfoTableEntry.KeyOffset = prevKeyOffset + prevKeySize;
            }

            // Check and update hashes (CRC32 with bitwise complement, aka reverse each bit)
            log.Info("Checking and updating hashes...");

            foreach (var entry in realEntries)
            {
                byte[] keyBytes = ExEncoding.UTF8.GetBytes(entry.Key);

                // Hash 1
                byte[] input1 = new byte[keyBytes.Length * 2];
                keyBytes.CopyTo(input1, 0);
                keyBytes.CopyTo(input1, keyBytes.Length);

                uint hash1 = ~Crc32Algorithm.Compute(input1);

                if (entry.InfoTableEntry.Hash1 != hash1)
                {
                    log.Info($"Hash1 doesn't match, using new hash\nOld hash: {entry.InfoTableEntry.Hash1:X04}\nNew hash: {hash1:X04}");
                    entry.InfoTableEntry.Hash1 = hash1;
                }

                // Hash 2
                byte[] input2 = new byte[keyBytes.Length * 3];
                keyBytes.CopyTo(input2, 0);
                keyBytes.CopyTo(input2, keyBytes.Length);
                keyBytes.CopyTo(input2, keyBytes.Length * 2);

                uint hash2 = ~Crc32Algorithm.Compute(input2);

                // If hash1 doesn't match then this hash obviously doesn't match as well since they have the same input (InfoTableEntry.Key)
                if (entry.InfoTableEntry.Hash2 != hash2)
                {
                    log.Info($"Hash2 doesn't match, using new hash\nOld hash: {entry.InfoTableEntry.Hash2:X04}\nNew hash: {hash2:X04}");
                    entry.InfoTableEntry.Hash2 = hash2;
                }
            }

            log.Info("Updating header...");

            // String Count
            log.Info("Current StringCount = " + _header.StringCount);
            _header.StringCount = Entries.Count;
            log.Info("New StringCount = " + _header.StringCount); // Key Count
            log.Info("Current KeyCount = " + _header.KeyCount);
            _header.KeyCount = realEntries.Count;
            log.Info("New KeyCount = " + _header.KeyCount);

            // StringBlockSize
            log.Info("Current StringBlockSize = " + _header.StringBlockSize);

            int newSize = 0;
            foreach (IGMD_Entry entry in Entries)
                newSize += ExEncoding.UTF8.GetByteCount(entry.Value) + 1; // +1 because szString

            _header.StringBlockSize = newSize;
            log.Info("New StringBlockSize = " + _header.StringBlockSize);

            // KeyBlockSize
            log.Info("Current KeyBlockSize = " + _header.KeyBlockSize);

            int lastKeySize = ExEncoding.UTF8.GetByteCount(realEntries.Last().Key) + 1; // +1 for szString end
            _header.KeyBlockSize = realEntries.Last().InfoTableEntry.KeyOffset + lastKeySize;

            log.Info("New KeyBlockSize = " + _header.KeyBlockSize);
        }

        /// <summary>
        /// Add string with key to the end of the list, unless an index is given.
        /// If an index is given the key/value will be added at that index, and all keys/values after that index will be moved one slot
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddString(string key, string value, int index = -1)
        {
            GMD_Entry newEntry = new GMD_Entry { Key = key, Value = value };

            if (index == -1)
                Entries.Add(newEntry);
            else
                Entries.Insert(index, newEntry);
        }

        /// <summary>
        /// Remove string with key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveString(string key)
        {
            var entry = Entries.OfType<GMD_Entry>().FirstOrDefault(x => x.Key == key);

            if (entry != null)
                Entries.Remove(entry);
        }
    }

    public interface IGMD_Entry
    {
        string Value { get; set; }
    }

    public class GMD_Entry : IGMD_Entry
    {
        public GMD_InfoTableEntry InfoTableEntry;
        public string Key { get; set; } = "CIRILLA_KEY";
        public string Value { get; set; } = "Value";
    }

    public class GMD_EntryWithoutKey : IGMD_Entry
    {
        public string Value { get; set; } = "Invalid Message";
    }
}
