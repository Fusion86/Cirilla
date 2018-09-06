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
        public List<string[]> Objects;

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

                // InfoBlocks
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

                // Object definitions
                uint keyBlockSize = Header.OffsetToData - (uint)fs.Position;
                Encoding enc = Encoding.GetEncoding("UTF-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback); // Encoding that throws errors

                Objects = new List<string[]>(Header.InfoBlockCount);
                for (int i = 0; i < Header.InfoBlockCount; i++)
                {
                    fs.Position = 0x18 + InfoBlocks[i].Header.KeyOffset; // 0x18 = sizeof FSM_Header

                    Objects.Add(new string[InfoBlocks[i].Header.StringCount]);
                    for (int j = 0; j < InfoBlocks[i].Header.StringCount; j++)
                    {
                        Objects[i][j] = br.ReadStringZero(Encoding.UTF8);
                    }
                }

                // CodeObjects
                FSM_CodeObjectContainer root = new FSM_CodeObjectContainer(br);
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

        public interface IFSM_CodeObject { }

        public class FSM_CodeObjectContainer : IFSM_CodeObject
        {
            // In the 'Root' CodeObject  | .Size = size in bytes
            // in every other CodeObject | .Size = number of children

            public FSM_CodeObjectHeader Header;
            public List<IFSM_CodeObject> Nodes;

            public FSM_CodeObjectContainer(BinaryReader br, bool isRoot = true)
            {
                Header = br.ReadStruct<FSM_CodeObjectHeader>();

                if (isRoot)
                {
                    // Contains CodeObjectContainers

                    br.BaseStream.Position += 4; // Skip zeros?
                    long posAfterHeader = br.BaseStream.Position;

                    Nodes = new List<IFSM_CodeObject>();
                    while (br.BaseStream.Position < posAfterHeader + Header.Size)
                    {
                        Nodes.Add(new FSM_CodeObjectContainer(br, false));
                    }
                }
                else
                {
                    // Contains CodeObjects

                    Nodes = new List<IFSM_CodeObject>(Header.Size);
                    for (int i = 0; i < Header.Size; i++)
                    {
                        break;
                    }
                }
            }
        }

        public class FSM_CodeObject : IFSM_CodeObject
        {
            public byte[] Magic;
            public byte[] Data;

            public FSM_CodeObject(BinaryReader br)
            {
                Magic = br.ReadBytes(4);
            }
        }
    }
}
