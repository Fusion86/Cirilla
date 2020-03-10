using Cirilla.Core.Extensions;
using Cirilla.Core.Interfaces;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cirilla.Core.Models
{
    public class MOD3 : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public MOD3_Header Header => _header;
        public List<string> MaterialNames { get; }
        public List<MOD3_MeshPart> MeshParts { get; }
        public List<VertexBuffer> VertexBuffers { get; }
        public List<TriFaceCollection> Faces { get; }

        private MOD3_Header _header;

        public MOD3(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                _header = br.ReadStruct<MOD3_Header>();

                if (_header.MagicString != "MOD")
                    throw new Exception("Not a MOD file!");

                // Material names
                MaterialNames = new List<string>(_header.MaterialCount);
                fs.Position = (long)_header.MaterialNamesOffset;

                for (int i = 0; i < _header.MaterialCount; i++)
                {
                    byte[] b = br.ReadBytes(128);
                    string name = Encoding.UTF8.GetString(b).TrimEnd('\0');
                    MaterialNames.Add(name);
                }

                // Mesh parts
                MeshParts = new List<MOD3_MeshPart>(_header.MeshCount);

                fs.Position = (long)_header.MeshOffset;
                for (int i = 0; i < _header.MeshCount; i++)
                    MeshParts.Add(br.ReadStruct<MOD3_MeshPart>());

                // Vertices
                VertexBuffers = new List<VertexBuffer>(_header.MeshCount);

                for (int i = 0; i < _header.MeshCount; i++)
                {
                    if (MeshParts[i].VertexCount > 0)
                    {
                        long boff = MeshParts[i].VertexSub + MeshParts[i].VertexBase;
                        fs.Position = (long)_header.VertexOffset + MeshParts[i].VertexOffset + MeshParts[i].BlockSize * boff;

                        switch (MeshParts[i].BlockType)
                        {
                            case 0xf637401c:
                                VertexBuffers.Add(ReadVertexBuffer<VertexBufferIASkin4wt1UV>(br, i));
                                break;
                            case 0x3c730760:
                                VertexBuffers.Add(ReadVertexBuffer<VertexBufferIASkin4wt1UVColor>(br, i));
                                break;
                            case 0x366995a7:
                                VertexBuffers.Add(ReadVertexBuffer<VertexBufferIASkin8wt1UVColor>(br, i));
                                break;
                            default:
                                Logger.Warn($"Unrecognized block type '{MeshParts[i].BlockType:X}' in MeshPart {i}");
                                break;
                        }
                    }
                }

                // Faces
                Faces = new List<TriFaceCollection>(_header.MeshCount);

                for (int i = 0; i < _header.MeshCount; i++)
                {
                    fs.Position = (long)_header.FacesOffset + MeshParts[i].FaceOffset * 2;
                    var col = new TriFaceCollection();
                    col.MeshPartIndex = i;
                    col.Faces = new TriFace[MeshParts[i].IndexCount / 3];
                    for (int j = 0; j < col.Faces.Length; j++)
                        col.Faces[j] = br.ReadStruct<TriFace>();
                    Faces.Add(col);
                }

                // TODO: Bones, matrices and other stuff
            }
        }

        private VertexBuffer ReadVertexBuffer<T>(BinaryReader br, int index) where T : IVertex
        {
            var buff = new VertexBuffer(index, MeshParts[index].VertexCount, typeof(T));
            for (int j = 0; j < MeshParts[index].VertexCount; j++)
                buff.Vertices[j] = br.ReadStruct<T>();
            return buff;
        }
    }

    public class VertexBuffer
    {
        public Type VertexType { get; }
        public int MeshPartIndex { get; }
        public IVertex[] Vertices { get; }

        public VertexBuffer(int index, int vertexCount, Type vertexType)
        {
            VertexType = vertexType;
            Vertices = new IVertex[vertexCount];
        }
    }

    public class TriFaceCollection
    {
        public int MeshPartIndex;
        public TriFace[] Faces;
    }
}
