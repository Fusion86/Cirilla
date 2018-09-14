using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;

namespace Cirilla.Core.Models
{
    public class NBSC : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public NBSC_Header Header { get; }
        public List<NBSC_NPC> Items { get; }

        public NBSC(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                Header = br.ReadStruct<NBSC_Header>();

                // Load NPCs
                Items = new List<NBSC_NPC>(Header.EntryCount);
                for (int i = 0; i < Header.EntryCount; i++)
                {
                    Items.Add(br.ReadStruct<NBSC_NPC>());
                }
            }
        }

        public void Save(string path)
        {
            Logger.Info($"Saving NBSC to '{path}'");

            using (FileStream fs = File.OpenWrite(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                // Header
                bw.Write(Header.ToBytes());

                foreach (var item in Items)
                {
                    bw.Write(item.ToBytes());
                }
            }
        }
    }
}
