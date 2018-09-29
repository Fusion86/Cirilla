using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NBSC_Header
    {
        public short Unk1;
        public short EntryCount;
        public short Zero;

        // 0x06
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NBSC_NPC
    {
        public int Id;
        public int Unk2;
        public int Unk3;
        public float Scale;

        // 0x10
        public int Unk5;
        public int Unk6;
        public int Unk7;
        public int Unk8;

        // 0x20
        public int Unk9;
        public int Unk10;
        public int Unk11;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk12;

        // 0x2F
    }
}
