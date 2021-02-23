using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BaseGameSaveData_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x01'00'00'00

        public uint Unk1;
        public uint Unk2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash;

        public long DataSize;
        public long SteamId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BaseGameSaveData_SaveSlot
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        public int HunterRank;
        public int Zeni;
        public int ResearchPoints;
        public int HunterXp;
        public int PlayTime; // In seconds

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 894493, ArraySubType = UnmanagedType.U1)]
        public byte[] NotImplemented1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] PalicoName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 113244, ArraySubType = UnmanagedType.U1)]
        public byte[] NotImplemented2;
    }
}
