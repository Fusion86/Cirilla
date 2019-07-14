using Cirilla.Core.Interfaces;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MOD3_Header
    {
        #region Native

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "MOD3"

        public byte Padding1;

        public byte Version1;
        public byte Version2;
        public short BoneCount;
        public short MeshCount;
        public short MaterialCount;
        public int VertextCount;

        // 0x10

        public int FaceCount;
        public int VertexIds;
        public int VertexBufferSize;
        public int SecondBuffersize;

        // 0x20

        public ulong GroupCount;
        public ulong BoneMapcount;
        public ulong BoneOffset;
        public ulong GroupOffset;

        // 0x40

        public ulong MaterialNamesOffset;
        public ulong MeshOffset;
        public ulong VertexOffset;
        public ulong FacesOffset;

        // 0x60

        public ulong UnkOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 216, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk;

        // 0x140

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MOD3_MeshPart
    {
        public short Unk1;
        public short VertexCount;
        public short VisibleCondition;
        public short MaterialIndex;
        public int Lod;
        public short Unk2;
        public byte BlockSize;
        public byte Unk3;

        // 0x10

        public int VertexSub;
        public int VertexOffset;
        public uint BlockType;
        public int FaceOffset;

        // 0x20

        public int IndexCount; // FaceCount in reference
        public int VertexBase;
        public byte BoneRemapId;

        // 0x29

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 39, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk4;

        // 0x50
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Position
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Normal
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;
    }

    // I don't actually understand what this is
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tangent
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Uv
    {
        public ushort X; // TODO: Make 16bit float
        public ushort Y; // TODO: Make 16bit float
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BaseWeight
    {
        public int Weight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExtendedWeight
    {
        public byte Weight0;
        public byte Weight1;
        public byte Weight2;
        public byte Weight3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorRGBA
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Alpha;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexBufferIASkin4wt1UV : IVertex, IVertexBaseWeight
    {
        private Position position;
        private Normal normal;
        private Tangent tangent;
        private Uv uv;
        private BaseWeight baseWeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        private byte[] bone;

        public Position Position => position;
        public Normal Normal => normal;
        public Tangent Tangent => tangent;
        public Uv Uv => uv;
        public BaseWeight BaseWeight => baseWeight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexBufferIASkin4wt1UVColor : IVertex, IVertexBaseWeight, IVertexColor
    {
        private Position position;
        private Normal normal;
        private Tangent tangent;
        private Uv uv;
        private BaseWeight baseWeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        private byte[] bone;

        private ColorRGBA color;

        public Position Position => position;
        public Normal Normal => normal;
        public Tangent Tangent => tangent;
        public Uv Uv => uv;
        public BaseWeight BaseWeight => baseWeight;
        public ColorRGBA Color => color;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexBufferIASkin8wt1UVColor : IVertex, IVertexBaseWeight, IVertexColor
    {
        private Position position;
        private Normal normal;
        private Tangent tangent;
        private Uv uv;
        private BaseWeight baseWeight;
        private ExtendedWeight ExtendedWeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U1)]
        private byte[] bone;

        private ColorRGBA color;

        public Position Position => position;
        public Normal Normal => normal;
        public Tangent Tangent => tangent;
        public Uv Uv => uv;
        public BaseWeight BaseWeight => baseWeight;
        public ColorRGBA Color => color;
    }

    public struct TriFace
    {
        public short Vertex1;
        public short Vertex2;
        public short Vertex3;
    }
}
