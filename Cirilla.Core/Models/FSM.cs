using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class FSM : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public FSM_Header Header;
        public List<long> InfoBlockOffsets;
        public List<FSM_InfoBlock> InfoBlocks;
        public List<string[]> Keys;

        public FSM(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                Header = br.ReadStruct<FSM_Header>();

                //long posAfterHeader = fs.Position; // Should be 0x18 (24)

                // InfoBlock offsets
                InfoBlockOffsets = new List<long>(Header.InfoBlockCount);
                for (int i = 0; i < Header.InfoBlockCount; i++)
                {
                    InfoBlockOffsets.Add(br.ReadInt64());
                }

                InfoBlocks = new List<FSM_InfoBlock>(Header.InfoBlockCount);
                for (int i = 0; i < Header.InfoBlockCount; i++)
                {
                    // Here fs.Position is (posAfterHeader + InfoBlockOffsets[i])

                    long bytesToRead;

                    if (i == Header.InfoBlockCount - 1)
                        bytesToRead = InfoBlocks[0].Header.KeyOffset - InfoBlockOffsets[i]; // If this is the last entry
                    else
                        bytesToRead = InfoBlockOffsets[i + 1] - InfoBlockOffsets[i]; // If this isn't the last entry

                    InfoBlocks.Add(new FSM_InfoBlock(br, (int)bytesToRead));
                }

                // Keys
                uint keyBlockSize = Header.OffsetToData - (uint)fs.Position;
                Encoding enc = Encoding.GetEncoding("UTF-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback); // Encoding that throws errors

                Keys = new List<string[]>(Header.InfoBlockCount);
                for (int i = 0; i < Header.InfoBlockCount; i++)
                {
                    fs.Position = 0x18 + InfoBlocks[i].Header.KeyOffset; // 0x18 = sizeof FSM_Header

                    Keys.Add(new string[InfoBlocks[i].Header.StringCount]);
                    for (int j = 0; j < InfoBlocks[i].Header.StringCount; j++)
                    {
                        Keys[i][j] = br.ReadStringZero(Encoding.UTF8);
                    }
                }
            }
        }

        public class FSM_InfoBlock
        {
            public FSM_InfoBlockHeader Header;
            public byte[] Data;

            public FSM_InfoBlock(BinaryReader br, int totalSize)
            {
                Header = br.ReadStruct<FSM_InfoBlockHeader>();

                int dataSize = totalSize - 0x20; // sizeof FSM_InfoBlockHeader
                Data = br.ReadBytes(dataSize);
            }
        }
    }
}
