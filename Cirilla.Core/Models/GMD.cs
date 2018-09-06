using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                Filename = Encoding.ASCII.GetString(bytes);
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
                    Keys.Add(Encoding.ASCII.GetString(bytes));
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

                    strings[i] = br.ReadStringZero(Encoding.UTF8);

                    if (strings[i] == "Invalid Message") invalidMessageCount++;
                }

                // Figure out which strings to load
                if (skipInvalidMessages)
                {
                    if (strings.Length - invalidMessageCount == Header.KeyCount)
                    {
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

            UpdateHeader();
            //UpdateEntries();

            Logger.Info("Writing bytes...");

            using (FileStream fs = File.OpenWrite(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(Header.ToBytes());
                bw.Write(Encoding.ASCII.GetBytes(Filename));
                bw.Write((byte)0); // szString end of string

                foreach (var item in Entries)
                {
                    bw.Write(item.ToBytes());
                }

                bw.Write(Unk1);

                foreach (var item in Keys)
                {
                    bw.Write(Encoding.ASCII.GetBytes(item));
                    bw.Write((byte)0); // szString end of string
                }

                foreach (var item in Strings)
                {
                    bw.Write(Encoding.UTF8.GetBytes(item));
                    bw.Write((byte)0); // szString end of string
                }
            }

            Logger.Info("Saved file!");
        }

        /// <summary>
        /// Update header
        /// This is needed when the strings changed (technically only if they all together are larger than before)
        /// </summary>
        public void UpdateHeader()
        {
            Logger.Info("Updating header...");

            // String Count
            Logger.Info("Current StringCount = " + Header.StringCount);
            Header.StringCount = (uint)Strings.Count;
            Logger.Info("New StringCount = " + Header.StringCount);

            // StringBlockSize
            Logger.Info("Current StringBlockSize = " + Header.StringBlockSize);

            uint newSize = 0;

            for (int i = 0; i < Header.StringCount; i++)
            {
                newSize += (uint)Encoding.UTF8.GetByteCount(Strings[i]) + 1; // +1 because szString
            }

            Header.StringBlockSize = newSize;
            Logger.Info("New StringBlockSize = " + Header.StringBlockSize);
        }

        /// <summary>
        /// Update entries
        /// This is only needed when the Tags changed
        /// </summary>
        public void UpdateEntries()
        {
            Logger.Info("Updating entries...");

            throw new NotImplementedException();
        }
    }
}
