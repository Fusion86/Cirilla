using Cirilla.Core.Enums;
using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class GMD
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public GMD_Header Header;
        public string Filename;
        public GMD_Entry[] Entries;
        public byte[] Unk1;
        public string[] Keys;
        public string[] Strings;

        private GMD()
        {

        }

        public static GMD Load(string path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                GMD gmd = new GMD();

                // Header
                gmd.Header = br.ReadStruct<GMD_Header>();

                if (gmd.Header.MagicString != "GMD") throw new Exception("Not a GMD file!");

                // Log some info about the GMD language
                if (Enum.IsDefined(typeof(EmLanguage), gmd.Header.Language))
                {
                    EmLanguage language = (EmLanguage)gmd.Header.Language;
                    Logger.Info("Language: " + language);
                }
                else
                {
                    Logger.Warn($"Unknown language: 0x{gmd.Header.Language:X04} ({gmd.Header.Language})");
                }

                // Filename
                byte[] bytes = br.ReadBytes((int)gmd.Header.FilenameLength); // Excludes \0 end of szString
                gmd.Filename = Encoding.ASCII.GetString(bytes);
                fs.Position++; // Skip the zero from the szString
                long posAfterFilename = fs.Position;

                // Entries table
                // Sometimes doesn't exist
                if (posAfterFilename + gmd.Header.KeyBlockSize + gmd.Header.StringBlockSize == fs.Length)
                {
                    // Doesn't exist, untested
                    fs.Position += gmd.Header.KeyCount + 2048 + gmd.Header.KeyBlockSize;
                }
                else
                {
                    // Exits
                    gmd.Entries = new GMD_Entry[gmd.Header.KeyCount];
                    for (int i = 0; i < gmd.Header.KeyCount; i++)
                    {
                        gmd.Entries[i] = br.ReadStruct<GMD_Entry>();
                    }
                }

                // Block with unkown data
                gmd.Unk1 = br.ReadBytes(0x800);

                // Tags, only exists if gmd.Entries exists
                if (gmd.Entries != null)
                {
                    gmd.Keys = new string[gmd.Header.StringCount];
                    for (int i = 0; i < gmd.Header.KeyCount; i++)
                    {
                        int szLength;
                        if (i == gmd.Header.KeyCount - 1)
                            szLength = (int)(gmd.Header.KeyBlockSize - gmd.Entries[i].KeyOffset);
                        else
                            szLength = (int)(gmd.Entries[i + 1].KeyOffset - gmd.Entries[i].KeyOffset);

                        bytes = br.ReadBytes(szLength - 1); // Don't read \0
                        fs.Position++; // Skip over \0
                        gmd.Keys[i] = Encoding.ASCII.GetString(bytes);
                    }
                }

                // Strings, seperated by \0 (aka normal szString)
                // TODO: Can probably optimize this
                gmd.Strings = new string[gmd.Header.StringCount];
                for (int i = 0; i < gmd.Header.StringCount; i++)
                {
                    byte b;
                    List<byte> szBytes = new List<byte>();

                    while (true)
                    {
                        b = br.ReadByte();

                        if (b == 0)
                        {
                            // Stop if we found a \0 **AND** we already have read some text.
                            // This is because a string could have empty space in front of it.
                            // While this is 'undocumented behaviour' it works in-game.
                            if (szBytes.Count > 0)
                                break;
                        }
                        else
                        {
                            szBytes.Add(b);
                        }
                    }

                    gmd.Strings[i] = Encoding.UTF8.GetString(szBytes.ToArray());
                }

                return gmd;
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
            Header.StringCount = (uint)Strings.Length;
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
