using Cirilla.Core.Helpers;
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
        public int Unk3;

        [Endian(Endianness.LittleEndian)]
        public int Unk4;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int InfoBlockCount;

        [Endian(Endianness.LittleEndian)]
        public int OffsetToData;

        // 0x18

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_InfoBlockHeader
    {
        [Endian(Endianness.LittleEndian)]
        public int Unk1;

        [Endian(Endianness.LittleEndian)]
        public int Unk2;

        [Endian(Endianness.LittleEndian)]
        public int StringCount;

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

        // 0x20
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_CodeObjectHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x01'00'00'00

        [Endian(Endianness.LittleEndian)]
        public int Size;
    }
}
