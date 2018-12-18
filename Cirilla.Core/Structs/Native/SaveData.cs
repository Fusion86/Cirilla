using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_Header
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
    public struct SaveData_SaveSlot
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        public int HunterRank;
        public int Zeni;
        public int ResearchPoints;
        public int HunterXp;
        public int PlayTime; // In seconds
        public int Unk1;

        public CharacterAppearance CharacterAppearance;
        public PalicoAppearance PalicoAppearance;

        public SaveData_GuildCard GuildCard;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.U1)]
        public SaveData_GuildCard[] CollectedGuildCards;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 106038, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 63224, ArraySubType = UnmanagedType.U1)]
        public byte[] ItemLoadouts;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 608, ArraySubType = UnmanagedType.U1)]
        public byte[] ItemPouch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11200, ArraySubType = UnmanagedType.U1)]
        public byte[] ItemBox;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 67992, ArraySubType = UnmanagedType.U1)]
        public byte[] EquipmentSlots;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 148032, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] PalicoName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 540, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk5;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10500, ArraySubType = UnmanagedType.U1)]
        public byte[] Investigations;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4025, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk6;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60928, ArraySubType = UnmanagedType.U1)]
        public byte[] EquipmentLoadout;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25889, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk7;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512, ArraySubType = UnmanagedType.U1)]
        public byte[] ClaimedDLC;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 469, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk8;

        public byte IsActiveSlot; // 0x80 (128) = yes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10383, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk9;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SaveData_GuildCard
    {
        public long SteamId;
        public long Created; // Timestamp
        public byte Unk1;
        public uint HunterRank;
        public uint PlayTime; // Seconds
        public long LastUpdate; // Timestamp

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] HunterName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 54, ArraySubType = UnmanagedType.U1)]
        public byte[] PrimaryGroup;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3;

        public CharacterAppearance Appearance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 208, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] PalicoName;

        public uint PalicoRank; // Actual rank minus 1?
        public uint PalicoHealth;
        public uint PalicoAttackMagic;
        public uint PalicoAttackRaw;
        public int PalicoAffinity;
        public uint PalicoDefense;
        public int PalicoFireResistance;
        public int PalicoWaterResistance;
        public int PalicoThunderResistance;
        public int PalicoIceResistance;
        public int PalicoDragonResistance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk6;

        public byte PalicoG1;
        public byte PalicoG2;
        public byte PalicoG3;
        public byte PalicoG4;
        public byte PalicoG5;
        public byte PalicoG6;
        public uint Unity;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk7;

        public short QuestsCompletedLowRank;
        public short QuestsCompletedHighRank;
        public short InvestigationsCompleted;
        public short ArenaQuestsCompleted;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 123, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Greeting;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U1)]
        public byte[] Title;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk9;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3020, ArraySubType = UnmanagedType.U1)]
        public byte[] ArenaStats;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U1)]
        public byte[] Captured;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U1)]
        public byte[] Slayed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U1)]
        public byte[] Largest;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.U1)]
        public byte[] Smallest;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.U1)]
        public byte[] Research;
    }
}
