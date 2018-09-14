using Cirilla.Core.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TSS_Header
    {
        [Endian(Endianness.LittleEndian)]
        public int Checksum; // Possibly?

        [Endian(Endianness.LittleEndian)]
        public int Unk1;

        [Endian(Endianness.LittleEndian)]
        public int DataSize;

        [Endian(Endianness.LittleEndian)]
        public int DataOffset;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int BlockCount;

        // 0x14
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct TSS_EventData
    {
        #region Native

        [Endian(Endianness.LittleEndian)]
        public int Unk1;

        [Endian(Endianness.LittleEndian)]
        public int Unk2;

        [Endian(Endianness.LittleEndian)]
        public int Unk3;

        // 0x0C

        [Endian(Endianness.LittleEndian)]
        public long StartTime; // UTC

        [Endian(Endianness.LittleEndian)]
        public long EndTime; // UTC

        // 0x1C

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk4;

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Start: {DateTimeOffset.FromUnixTimeSeconds(StartTime)}    End: {DateTimeOffset.FromUnixTimeSeconds(EndTime)}";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TSS_Block5Chunk_Header
    {
        [Endian(Endianness.LittleEndian)]
        public int Unknown;

        [Endian(Endianness.LittleEndian)]
        public int FileCount;

        [Endian(Endianness.LittleEndian)]
        public int DataSize;
    }
}