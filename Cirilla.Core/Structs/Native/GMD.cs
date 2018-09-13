using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GMD_Header
    {
        #region Native

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "GMD"

        public byte Padding1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Version; // Version?

        [Endian(Endianness.LittleEndian)]
        public int Language;

        [Endian(Endianness.LittleEndian)]
        public int Unk1; // Zero

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int Unk2; // Zero

        [Endian(Endianness.LittleEndian)]
        public int KeyCount;

        [Endian(Endianness.LittleEndian)]
        public int StringCount; // Usually the same as KeyCount

        [Endian(Endianness.LittleEndian)]
        public int KeyBlockSize;

        // 0x20

        [Endian(Endianness.LittleEndian)]
        public int StringBlockSize;

        [Endian(Endianness.LittleEndian)]
        public int FilenameLength;

        // 0x28

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GMD_InfoTableEntry
    {
        [Endian(Endianness.LittleEndian)]
        public int Index;

        [Endian(Endianness.LittleEndian)]
        public int Unk2;

        [Endian(Endianness.LittleEndian)]
        public int Unk3;

        [Endian(Endianness.LittleEndian)]
        public int Unk4;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int KeyOffset;

        [Endian(Endianness.LittleEndian)]
        public int Unk6;

        [Endian(Endianness.LittleEndian)]
        public int Unk7;

        [Endian(Endianness.LittleEndian)]
        public int Unk8;
    }
}
