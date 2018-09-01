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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "GMD"

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk0; // Version?

        [Endian(Endianness.LittleEndian)]
        public UInt32 Language;

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk1; // Zero

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public UInt32 Unk2; // Zero

        [Endian(Endianness.LittleEndian)]
        public UInt32 NumOfStringIds;

        [Endian(Endianness.LittleEndian)]
        public UInt32 NumOfStrings; // Should match NumOfStringIds

        [Endian(Endianness.LittleEndian)]
        public UInt32 LengthOfStringIdNames;

        // 0x20

        [Endian(Endianness.LittleEndian)]
        public UInt32 LengthOfStrings;

        [Endian(Endianness.LittleEndian)]
        public UInt32 FilenameLength;

        // Filename starts at 0x28

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }
}
