using Cirilla.Core.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_Header
    {
        #region Native

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "GMD"

        public byte Padding1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Version;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk3;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk4;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int InfoBlockCount;

        [Endian(Endianness.LittleEndian)]
        public UInt32 OffsetToData;

        // 0x18

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_InfoBlockHeader
    {
        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk1;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk2;

        [Endian(Endianness.LittleEndian)]
        public UInt32 StringCount;

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

        // 0x20
    }
}
