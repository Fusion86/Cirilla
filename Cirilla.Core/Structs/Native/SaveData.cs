using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x01'00'00'00

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.U1)]
        public byte[] Hash;
        public long DataSize;
        public long SteamId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk2;
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 497223, ArraySubType = UnmanagedType.U1)]
        public byte[] GuildCards;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 510413, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SaveData_Appearance
    {
        // All colors are ABGR

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] MakeUp2Color;
        public float MakeUp2PosX;
        public float MakeUp2PosY;
        public float MakeUp2SizeX;
        public float MakeUp2SizeY;
        public float MakeUp2Glossy;
        public float MakeUp2Metallic;
        public int MakeUp2Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] MakeUp1Color;
        public float MakeUp1PosX;
        public float MakeUp1PosY;
        public float MakeUp1SizeX;
        public float MakeUp1SizeY;
        public float MakeUp1Glossy;
        public float MakeUp1Metallic;
        public int MakeUp1Type;

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
}
