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

        public SaveData_Appearance Appearance;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 44, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2;

        public SaveData_GuildCard GuildCard;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.U1)]
        public SaveData_GuildCard[] CollectedGuildCards;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 510413, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_Appearance
    {
        // All colors are ABGR

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Makeup2Color;

        public float Makeup2PosX;
        public float Makeup2PosY;
        public float Makeup2SizeX;
        public float Makeup2SizeY;
        public float Makeup2Glossy;
        public float Makeup2Metallic;
        public int Makeup2Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Makeup1Color;

        public float Makeup1PosX;
        public float Makeup1PosY;
        public float Makeup1SizeX;
        public float Makeup1SizeY;
        public float Makeup1Glossy;
        public float Makeup1Metallic;
        public int Makeup1Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] LeftEyeColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] RightEyeColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] EyebrowColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] FacialHairColor;

        public byte EyeWidth;
        public byte EyeHeight;
        public byte SkinColorX;
        public byte SkinColorY;
        public byte Age;
        public byte Wrinkles;
        public byte NoseHeight;
        public byte MouthHeight;
        public int Gender; // 0 = male, 1 = female
        public byte BrowType;
        public byte FaceType;
        public byte EyeType;
        public byte NoseType;
        public byte MouthType;
        public byte EyebrowType;
        public byte EyelashLength; // 0 = short, 1 = average, 2 = long
        public byte FacialHairType;
        public int Unk1; // Unused?

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] HairColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] ClothingColor;

        public short HairType;
        public byte ClothingType;
        public byte Voice;
        public int Expression;
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

        public SaveData_Appearance Appearance;

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
