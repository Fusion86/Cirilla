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

        public GMD_Header Header;
        public string Filename;
        public List<GMD_Entry> Entries;
        public byte[] Unk1;
        public List<string> Keys;
        public List<string> Strings;

        public GMD(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                Header = br.ReadStruct<GMD_Header>();

                if (Header.MagicString != "GMD") throw new Exception("Not a GMD file!");

                // Log some info about the GMD language
                if (Enum.IsDefined(typeof(EmLanguage), Header.Language))
                {
                    EmLanguage language = (EmLanguage)Header.Language;
                    Logger.Info("Language: " + language);
                }
                else
                {
                    Logger.Warn($"Unknown language: 0x{Header.Language:X04} ({Header.Language})");
                }

                // Skip "Invalid Message" strings when StringCount > KeyCount
                bool skipInvalidMessages = Header.StringCount > Header.KeyCount;

                if (skipInvalidMessages)
                    Logger.Info("skipInvalidMessages is enabled");

                // Filename
                byte[] bytes = br.ReadBytes((int)Header.FilenameLength); // Excludes \0 end of szString
                Filename = ExEncoding.ASCII.GetString(bytes);
                fs.Position++; // Skip the zero from the szString
                long posAfterFilename = fs.Position;

                // Entries table
                Entries = new List<GMD_Entry>((int)Header.KeyCount);
                for (int i = 0; i < Header.KeyCount; i++)
                {
                    Entries.Add(br.ReadStruct<GMD_Entry>());
                }

                // Block with unkown data
                Unk1 = br.ReadBytes(0x800);

                // Keys
                Keys = new List<string>((int)Header.KeyCount);
                for (int i = 0; i < Header.KeyCount; i++)
                {
                    int szLength;
                    if (i == Header.KeyCount - 1)
                        szLength = (int)(Header.KeyBlockSize - Entries[i].KeyOffset);
                    else
                        szLength = (int)(Entries[i + 1].KeyOffset - Entries[i].KeyOffset);

                    bytes = br.ReadBytes(szLength - 1); // Don't read \0
                    fs.Position++; // Skip over \0
                    Keys.Add(ExEncoding.ASCII.GetString(bytes));
                }

                // Strings
                int invalidMessageCount = 0;
                string[] strings = new string[Header.StringCount];

                // Read all strings, including "Invalid Message" strings
                // stop when we read all messages OR when we read the whole file
                for (int i = 0; i < Header.StringCount; i++)
                {
                    if (fs.Position == fs.Length)
                    {
                        Logger.Warn("We expected more strings, but we already are at the end of the stream.");
                        break;
                    }

                    strings[i] = br.ReadStringZero(ExEncoding.UTF8);

                    if (strings[i] == "Invalid Message") invalidMessageCount++;
                }

                // Figure out which strings to load
                if (skipInvalidMessages)
                {
                    if (strings.Length - invalidMessageCount == Header.KeyCount)
                    {
                        Logger.Info($"Skipped {invalidMessageCount} invalid messages");

                        // Don't load "Invalid Message" strings
                        Strings = strings.Where(x => x != "Invalid Message").ToList();
                    }
                    else
                    {
                        // In some rare cases we do want to load the "Invalid Message" strings even if skipInvalidMessages is enabled (e.g. chunk0\common\text\action_trial_ara.gmd)
                        // however in that case we end up with more strings than {Header.KeyCount} so we just skip the ones that don't have a key (last ones)
                        // This ususally happends to the languages: ara, kor, cht, pol, ptb, rus

                        Logger.Warn("Using weird workaround to load file, this file is probably not used in-game.");

                        Strings = strings.Take((int)Header.KeyCount).ToList();
                    }
                }
                else
                {
                    // Load all strings
                    Strings = new List<string>(strings);
                }
            }
        }

        public void Save(string path)
        {
            Logger.Info($"Saving {Filename} to '{path}'");

            UpdateEntries();
            UpdateHeader();

            Logger.Info("Writing bytes...");

            using (FileStream fs = File.OpenWrite(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(Header.ToBytes());
                bw.Write(ExEncoding.ASCII.GetBytes(Filename));
                bw.Write((byte)0); // szString end of string

                foreach (var item in Entries)
                {
                    bw.Write(item.ToBytes());
                }

                bw.Write(Unk1);

                foreach (var item in Keys)
                {
                    bw.Write(ExEncoding.ASCII.GetBytes(item));
                    bw.Write((byte)0); // szString end of string
                }

                foreach (var item in Strings)
                {
                    if (item == null) continue;

                    bw.Write(ExEncoding.UTF8.GetBytes(item));
                    bw.Write((byte)0); // szString end of string
                }
            }

            Logger.Info("Saved file!");
        }

        /// <summary>
        /// Update header
        /// This is needed when anything changes
        /// </summary>
        public void UpdateHeader()
        {
            Logger.Info("Updating header...");

            // String Count
            Logger.Info("Current StringCount = " + Header.StringCount);
            Header.StringCount = (uint)Strings.Count;
            Logger.Info("New StringCount = " + Header.StringCount);

            // Key Count
            Logger.Info("Current KeyCount = " + Header.KeyCount);
            Header.KeyCount = (uint)Keys.Count;
            Logger.Info("New KeyCount = " + Header.KeyCount);

            // StringBlockSize
            Logger.Info("Current StringBlockSize = " + Header.StringBlockSize);

            uint newSize = 0;

            for (int i = 0; i < Header.StringCount; i++)
            {
                if (Strings[i] != null)
                    newSize += (uint)ExEncoding.UTF8.GetByteCount(Strings[i]) + 1; // +1 because szString
            }

            Header.StringBlockSize = newSize;
            Logger.Info("New StringBlockSize = " + Header.StringBlockSize);

            // KeyBlockSize
            Logger.Info("Current KeyBlockSize = " + Header.KeyBlockSize);

            // ASCII.GetByteCount() is not needed because all chars are exactly one byte large
            Header.KeyBlockSize = Entries[Entries.Count - 1].KeyOffset + (uint)Keys[Keys.Count - 1].Length + 1; // +1 for szString end

            Logger.Info("New KeyBlockSize = " + Header.KeyBlockSize);
        }

        /// <summary>
        /// Update entries
        /// This is only needed when the Tags changed
        /// </summary>
        public void UpdateEntries()
        {
            Logger.Info("Updating entries...");

            List<GMD_Entry> newEntries = new List<GMD_Entry>(Keys.Count);
            newEntries.Add(Entries[0]); // First entry never changes (at least the offset + index, and we don't know what the other values mean)

            for (int i = 1; i < Keys.Count; i++) // Start at 1
            {
                // TODO: This doesn't set the Unk fields! But everything still seems to work just fine?
                Logger.Info("Creating new GMD_Entry for " + Keys[i - 1]);
                GMD_Entry newEntry = new GMD_Entry(); // Create new entry, this happens when we added a new key

                newEntry.Index = (uint)i;
                newEntry.KeyOffset = newEntries[i - 1].KeyOffset + (uint)Keys[i - 1].Length + 1; // +1 for szString end
                newEntries.Add(newEntry);
            }

            Entries = newEntries;
        }

        /// <summary>
        /// Add string with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddString(string key, string value, int index = -1)
        {
            if (Keys.Contains(key))
                throw new Exception("Can't add duplicate key!");

            if (index == -1)
            {
                Keys.Add(key);
                Strings.Add(value);
            }
            else
            {
                Keys.Insert(index, key);
                Strings.Insert(index, value);
            }
        }

        /// <summary>
        /// Remove string with key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveString(string key)
        {
            int idx = Keys.IndexOf(key);

            if (idx == -1)
                throw new Exception($"No string found with key '{key}'");

            Keys.RemoveAt(idx);
            Strings.RemoveAt(idx);
        }
    }
}
