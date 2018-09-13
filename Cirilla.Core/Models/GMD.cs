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
        public Dictionary<string, string> Entries;
        
        private byte[] Unk1;

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

                // Filename
                Filename = br.ReadStringZero(ExEncoding.ASCII);
                
                // Set Entries initial capacity
                Entries = new Dictionary<string, string>(Header.KeyCount);

                // Info Table
                GMD_InfoTableEntry[] infoTable = new GMD_InfoTableEntry[Header.KeyCount];
                
                for (int i = 0; i < Header.KeyCount; i++)
                {
                    infoTable[i] = br.ReadStruct<GMD_InfoTableEntry>();
                }

                // Block with unknown data
                Unk1 = br.ReadBytes(0x800);

                // Keys
                string[] keys = new string[Header.KeyCount];
                for (int i = 0; i < Header.KeyCount; i++)
                    keys[i] = br.ReadStringZero(ExEncoding.ASCII);

                // Strings
                for (int i = 0; i < Header.StringCount; i++)
                {
                    if (fs.Position == fs.Length)
                    {
                        Logger.Warn("We expected more strings, but we already are at the end of the stream.");
                        break;
                    }

                    Entries.Add(keys[i], br.ReadStringZero(ExEncoding.UTF8));
                }
            }
        }

        public void Save(string path)
        {
            Logger.Info($"Saving {Filename} to '{path}'");

            Logger.Info("Writing bytes...");

            using (FileStream fs = File.OpenWrite(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(Header.ToBytes());
                bw.Write(ExEncoding.ASCII.GetBytes(Filename));
                bw.Write((byte)0); // szString end of string

                // Info Table
                int idx = 0;
                int keyOffset = 0;
                foreach (var entry in Entries)
                {
                    // Generate new entry
                    GMD_InfoTableEntry infoTableEntry = new GMD_InfoTableEntry();
                    infoTableEntry.Index = idx;
                    infoTableEntry.KeyOffset = keyOffset;
                    
                    bw.Write(infoTableEntry.ToBytes());

                    idx++;
                    keyOffset += entry.Key.Length + 1; // +1 for szString terminator
                }

                bw.Write(Unk1);

                // Keys
                foreach (var entry in Entries)
                {
                    bw.Write(ExEncoding.ASCII.GetBytes(entry.Key));
                    bw.Write((byte)0); // szString end of string
                }

                // Strings
                foreach (var entry in Entries)
                {
                    bw.Write(ExEncoding.UTF8.GetBytes(entry.Value));
                    bw.Write((byte)0); // szString end of string
                }
            }

            Logger.Info("Saved file!");
        }

        /// <summary>
        /// Add string with key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddString(string key, string value, int index = -1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove string with key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveString(string key)
        {
            throw new NotImplementedException();
        }
    }
}
