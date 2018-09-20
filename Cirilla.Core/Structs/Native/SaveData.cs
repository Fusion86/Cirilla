using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x01'00'00'00

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk1;

        [Endian(Endianness.LittleEndian)]
        public long DataSize;

        [Endian(Endianness.LittleEndian)]
        public long SteamId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_SaveSlot
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        [Endian(Endianness.LittleEndian)]
        public int HunterRank;

        [Endian(Endianness.LittleEndian)]
        public int Zeni;

        [Endian(Endianness.LittleEndian)]
        public int ResearchPoints;

        [Endian(Endianness.LittleEndian)]
        public int HunterXp;

        [Endian(Endianness.LittleEndian)]
        public int PlayTime; // In seconds

        [Endian(Endianness.LittleEndian)]
        public int Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.U1)]
        public byte[] Appearance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 44, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 497223, ArraySubType = UnmanagedType.U1)]
        public byte[] GuildCards;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 510413, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3;
    }
}
