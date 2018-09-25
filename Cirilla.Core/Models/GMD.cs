using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cirilla.Core.Models
{
    public class GMD : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public GMD_Header Header => _header;
        public string Filename { get; }
        public List<IGMD_Entry> Entries { get; }

        private GMD_Header _header;
        private byte[] _unk1;

        public GMD(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                _header = br.ReadStruct<GMD_Header>();

                if (_header.MagicString != "GMD") throw new Exception("Not a GMD file!");

                // Log some info about the GMD language
                if (Enum.IsDefined(typeof(EmLanguage), _header.Language))
                {
                    EmLanguage language = (EmLanguage)_header.Language;
                    Logger.Info("Language: " + language);
                }
                else
                {
                    Logger.Warn($"Unknown language: 0x{_header.Language:X04} ({_header.Language})");
                }

                // Filename
                Filename = br.ReadStringZero(ExEncoding.ASCII);

                // Set Entries initial capacity
                Entries = new List<IGMD_Entry>(_header.StringCount);

                // Info Table
                for (int i = 0; i < _header.KeyCount; i++)
                {
                    GMD_Entry entry = new GMD_Entry { InfoTableEntry = br.ReadStruct<GMD_InfoTableEntry>() };

                    int lastStringIndex = 0;
                    if (Entries.OfType<GMD_Entry>().Count() > 0)
                        lastStringIndex = Entries.OfType<GMD_Entry>().Last().InfoTableEntry.StringIndex;

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
                {
                    entry.Key = br.ReadStringZero(ExEncoding.ASCII);
                }

                // Strings
                string[] strings = new string[_header.StringCount];
                long startOfStringBlock = fs.Position;

                for (int i = 0; i < _header.StringCount; i++)
                {
                    if (fs.Position == fs.Length)
                    {
                        Logger.Warn($"Expected to read {_header.StringCount - i} more strings (for a total of {_header.StringCount}). But we already are at the end of the stream!");
                        break;
                    }

                    strings[i] = br.ReadStringZero(ExEncoding.UTF8);
                }

                Logger.Info("Expected StringBlockSize = " + _header.StringBlockSize);
                Logger.Info("Actual StringBlockSize = " + (fs.Position - startOfStringBlock));

                if (_header.StringBlockSize != (fs.Position - startOfStringBlock))
                    Logger.Warn("Actual StringBlockSize is not the same as the expected StringBlockSize!");

                for (int i = 0; i < _header.StringCount; i++)
                    Entries[i].Value = strings[i];
            }
        }

        public void Save(string path)
        {
            Logger.Info($"Saving {Filename} to '{path}'");

            Update();

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                Logger.Info("Writing bytes...");

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
                    bw.Write(ExEncoding.ASCII.GetBytes(entry.Key));
                    bw.Write((byte)0); // szString end of string
                }

                // Strings, __includes__ GMD_EntryWithoutKey
                foreach (IGMD_Entry entry in Entries)
                {
                    bw.Write(ExEncoding.UTF8.GetBytes(entry.Value));
                    bw.Write((byte)0); // szString end of string
                }
            }

            Logger.Info("Saved file!");
        }

        /// <summary>
        /// Update header
        /// This is needed when anything changes
        /// </summary>
        private void Update()
        {
            Logger.Info("Updating entries...");

            var realEntries = Entries.OfType<GMD_Entry>().ToList();

            // First info entry always has Index = 0 and KeyOffset = 0
            realEntries[0].InfoTableEntry.StringIndex = 0;
            realEntries[0].InfoTableEntry.KeyOffset = 0;

            for (int i = 1; i < realEntries.Count; i++) // Start at 1
            {
                realEntries[i].InfoTableEntry.StringIndex = Entries.IndexOf(realEntries[i]);
                realEntries[i].InfoTableEntry.KeyOffset = realEntries[i - 1].InfoTableEntry.KeyOffset + realEntries[i - 1].Key.Length + 1; // +1 for szString end
            }

            Logger.Info("Updating header...");

            // String Count
            Logger.Info("Current StringCount = " + _header.StringCount);
            _header.StringCount = Entries.Count;
            Logger.Info("New StringCount = " + _header.StringCount);            // Key Count
            Logger.Info("Current KeyCount = " + _header.KeyCount);
            _header.KeyCount = realEntries.Count;
            Logger.Info("New KeyCount = " + _header.KeyCount);

            // StringBlockSize
            Logger.Info("Current StringBlockSize = " + _header.StringBlockSize);

            int newSize = 0;
            foreach (IGMD_Entry entry in Entries)
            {
                newSize += ExEncoding.UTF8.GetByteCount(entry.Value) + 1; // +1 because szString
            }

            _header.StringBlockSize = newSize;
            Logger.Info("New StringBlockSize = " + _header.StringBlockSize);

            // KeyBlockSize
            Logger.Info("Current KeyBlockSize = " + _header.KeyBlockSize);

            // ASCII.GetByteCount() is not needed because all chars are exactly one byte large
            _header.KeyBlockSize = realEntries.Last().InfoTableEntry.KeyOffset + realEntries.Last().Key.Length + 1; // +1 for szString end

            Logger.Info("New KeyBlockSize = " + _header.KeyBlockSize);
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
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class GMD_EntryWithoutKey : IGMD_Entry
    {
        public string Value { get; set; }
    }
}
