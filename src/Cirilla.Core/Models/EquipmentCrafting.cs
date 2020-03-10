using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;

namespace Cirilla.Core.Models
{
    public class EquipmentCrafting : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public EquipmentCrafting_Header Header => _header;
        public List<EquipmentCrafting_Entry> Entries;

        private EquipmentCrafting_Header _header;

        public EquipmentCrafting(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                _header = br.ReadStruct<EquipmentCrafting_Header>();

                Logger.Info("EntryCount: " + _header.EntryCount);

                Entries = new List<EquipmentCrafting_Entry>((int)_header.EntryCount);

                for (int i = 0; i < _header.EntryCount; i++)
                    Entries.Add(br.ReadStruct<EquipmentCrafting_Entry>());
            }
        }

        public void Save(string path)
        {
            Logger.Info($"Saving to '{path}'");

            Update();

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                Logger.Info("Writing bytes...");

                bw.Write(_header.ToBytes());

                foreach (var entry in Entries)
                    bw.Write(entry.ToBytes());
            }
        }

        private void Update()
        {
            Logger.Info("Updating header...");

            Logger.Info("Current EntryCount = " + _header.EntryCount);
            _header.EntryCount = (uint)Entries.Count;
            Logger.Info("new EntryCount = " + _header.EntryCount);
        }
    }
}
