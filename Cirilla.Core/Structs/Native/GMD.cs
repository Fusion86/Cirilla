using Cirilla.Core.Helpers;
using System;
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
        public UInt32 Language;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk1; // Zero

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk2; // Zero

        [Endian(Endianness.LittleEndian)]
        public UInt32 KeyCount;

        [Endian(Endianness.LittleEndian)]
        public UInt32 StringCount; // Usually the same as KeyCount

        [Endian(Endianness.LittleEndian)]
        public UInt32 KeyBlockSize;

        // 0x20

        [Endian(Endianness.LittleEndian)]
        public UInt32 StringBlockSize;

        [Endian(Endianness.LittleEndian)]
        public UInt32 FilenameLength;

        // Filename starts at 0x28

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GMD_Entry
    {
        [Endian(Endianness.LittleEndian)]
        public UInt32 Index;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk2;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk3;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk4;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public UInt32 KeyOffset;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk6;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk7;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk8;
    }
}
