using System.Collections.Generic;
using System.IO;
using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;

namespace Cirilla.Core.Models
{
    // See https://pastebin.com/raw/xhAr1YvL
    // Oh boy optimizing this class is going to be so much fun - not joking - but it probably isn't needed :(
    public class TSS : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public TSS_Header Header => _header;
        public List<ITSS_Block> Blocks { get; }

        private TSS_Header _header;

        public TSS(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            // We could probably decrypt the stream on the fly, but this works for now
            byte[] bytes = File.ReadAllBytes(path);
            DecryptData(ref bytes);

            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                // Header
                _header = br.ReadStruct<TSS_Header>();

                // Block sizes
                int[] blockSize = new int[_header.BlockCount];
                for (int i = 0; i < _header.BlockCount; i++)
                {
                    blockSize[i] = br.ReadInt32();
                }

                // Skip over padding
                ms.Position = _header.DataOffset;

                // Set init blocks capacity
                Blocks = new List<ITSS_Block>(_header.BlockCount);

                // Load blocks
                for (int i = 0; i < _header.BlockCount; i++)
                {
                    bytes = br.ReadBytes(blockSize[i]);

                    // Do something depending on the (expected) block type
                    if (i == 0)
                    {
                        Blocks.Add(new TSS_Block1(bytes));
                    }
                    else if (i == 4)
                    {
                        Blocks.Add(new TSS_Block5(bytes));
                    }
                    else
                    {
                        Blocks.Add(new TSS_BlockUnk(bytes));
                    }
                }
            }
        }

        private static void DecryptData(ref byte[] arr)
        {
            byte key = 0x0A; // IV
            for (int p = 0x0C; p < arr.Length; p++)
            {
                byte tmp = arr[p];
                arr[p] ^= key;
                key = tmp;
            }
        }
    }

    public interface ITSS_Block { }

    public class TSS_BlockUnk : ITSS_Block
    {
        public byte[] Data { get; }

        public TSS_BlockUnk(byte[] data)
        {
            Data = data;
        }
    }

    public class TSS_Block1 : ITSS_Block
    {
        public int EventCount { get; }
        public List<TSS_EventData> EventData { get; }

        public TSS_Block1(in byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, false))
            using (BinaryReader br = new BinaryReader(ms))
            {
                EventCount = br.ReadInt32();

                EventData = new List<TSS_EventData>(EventCount);
                for (int i = 0; i < EventCount; i++)
                {
                    EventData.Add(br.ReadStruct<TSS_EventData>());
                }
            }
        }
    }

    public class TSS_Block5 : ITSS_Block
    {
        public int QuestCount { get; }
        public List<TSS_Block5Chunk> Chunks { get; }

        public TSS_Block5(in byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, false))
            using (BinaryReader br = new BinaryReader(ms))
            {
                QuestCount = br.ReadInt32();

                Chunks = new List<TSS_Block5Chunk>(QuestCount);
                for (int i = 0; i < QuestCount; i++)
                {
                    TSS_Block5Chunk chunk = new TSS_Block5Chunk(br.ReadStruct<TSS_Block5Chunk_Header>());

                    for (int j = 0; j < chunk.Header.FileCount; j++)
                    {
                        int fileSize = br.ReadInt32();
                        byte[] bytes = br.ReadBytes(fileSize);

                        chunk.Files.Add(new TSS_Block5ChunkFile(fileSize, bytes));
                    }

                    Chunks.Add(chunk);
                }
            }
        }
    }

    public class TSS_Block5Chunk
    {
        public TSS_Block5Chunk_Header Header { get; }
        public List<TSS_Block5ChunkFile> Files { get; }

        public TSS_Block5Chunk(TSS_Block5Chunk_Header header)
        {
            Header = header;
            Files = new List<TSS_Block5ChunkFile>(Header.FileCount);
        }
    }

    public class TSS_Block5ChunkFile
    {
        public int FileSize { get; }
        public byte[] Data { get; }

        public TSS_Block5ChunkFile(int fileSize, byte[] data)
        {
            FileSize = fileSize;
            Data = data;
        }
    }
}