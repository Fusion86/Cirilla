//
// See misc/savedata.bt
//

using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SaveData_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x01'00'00'00

        public uint Unk1;
        public uint Unk2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash;

        /// <remarks>
        /// Base Game Data Size = 9438368
        /// Iceborne Data Size = 11284640
        /// </remarks>
        public long DataSize;

        public long SteamId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SaveData_SaveSlot
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        public int HunterRank;
        public int MasterRank;
        public int Zeni;
        public int ResearchPoints;
        public int HunterXp;
        public int MasterXp;
        public int PlayTime; // In seconds

        public CharacterAppearance CharacterAppearance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 382, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk3;

        public PalicoAppearance PalicoAppearance;

        /// <summary>
        /// Your own GuildCard.
        /// </summary>
        public SaveData_GuildCard GuildCard;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.U1)]
        public SaveData_GuildCard[] CollectedGuildCards;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 969962, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] PalicoName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 237679, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk5;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96, ArraySubType = UnmanagedType.U1)]
        public Shoutout[] Shoutouts;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129218, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SaveData_GuildCard
    {
        public long SteamId;
        public long Created; // Timestamp
        private byte Unk1;
        public int HunterRank;
        public int MasterRank;
        public long LastUpdate; // Timestamp
        private uint Zero1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 54, ArraySubType = UnmanagedType.U1)]
        public byte[] PrimaryGroup;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk2;

        public CharacterAppearance CharacterAppearance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 212, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] PalicoName;

        public uint PalicoRank; // Actual rank minus 1?

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 338, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Greeting;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Title;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6318, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk5;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Shoutout
    {
        // I don't think the game actually uses this value.
        public int Length;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = UnmanagedType.U1)]
        public byte[] Value;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 88, ArraySubType = UnmanagedType.U1)]
        private byte[] Unk5;
    }
}
