using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;

namespace Cirilla.Core.Models
{
    public class FSM : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public FSM_Header Header;
        public List<ulong> InfoBlockOffsets;

        public FSM(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                Header = br.ReadStruct<FSM_Header>();

                // InfoBlock offsets
                InfoBlockOffsets = new List<ulong>((int)Header.InfoBlockCount);
                for (int i = 0; i < Header.InfoBlockCount; i++)
                {
                    InfoBlockOffsets.Add(br.ReadUInt64());
                }
            }
        }
    }
}
