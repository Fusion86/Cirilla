using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct NBSC_Header
    {
        [Endian(Endianness.LittleEndian)]
        public short Unk1;

        [Endian(Endianness.LittleEndian)]
        public short EntryCount;

        [Endian(Endianness.LittleEndian)]
        public short Zero;

        // 0x06
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NBSC_NPC
    {
        [Endian(Endianness.LittleEndian)]
        public int Id;

        [Endian(Endianness.LittleEndian)]
        public int Unk2;

        [Endian(Endianness.LittleEndian)]
        public int Unk3;
        
        [Endian(Endianness.LittleEndian)]
        public float Scale;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int Unk5;

        [Endian(Endianness.LittleEndian)]
        public int Unk6;

        [Endian(Endianness.LittleEndian)]
        public int Unk7;

        [Endian(Endianness.LittleEndian)]
        public int Unk8;

        // 0x20

        [Endian(Endianness.LittleEndian)]
        public int Unk9;

        [Endian(Endianness.LittleEndian)]
        public int Unk10;

        [Endian(Endianness.LittleEndian)]
        public int Unk11;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk12;

        // 0x2F
    }
}
