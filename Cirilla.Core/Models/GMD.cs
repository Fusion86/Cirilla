using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
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

                // Skip "Invalid Message" strings when KeyCount != StringCount
                bool skipInvalidMessages = Header.KeyCount != Header.StringCount;

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

                // Tags
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
                Strings = new List<string>((int)Header.KeyCount);

                int skippedInvalidMessages = 0;
                for (int i = 0; i < Header.KeyCount; i++)
                {
                    if (fs.Position == fs.Length)
                    {
                        Logger.Warn("We expected more strings, but we already are at the end of the stream.");
                        break;
                    }

                    while(true)
                    {
                        string str = Utility.ReadZeroTerminatedString(br, Encoding.UTF8);

                        if (skipInvalidMessages && str == "Invalid Message")
                        {
                            // Ignore string and read next one
                            skippedInvalidMessages++;
                        }
                        else
                        {
                            // Save string and continue with next key
                            Strings.Add(str);
                            break;
                        }
                    }
                }

                if (skipInvalidMessages)
                {
                    uint expectedMessagesToSkip = Header.StringCount - Header.KeyCount;
                    Logger.Info($"Skipped {skippedInvalidMessages} invalid messages, expected to skip {expectedMessagesToSkip} invalid messages");

                    if (skippedInvalidMessages != expectedMessagesToSkip)
                        Logger.Warn("skippedInvalidMessages != expectedMessagesToSkip, that can't be good!");
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
