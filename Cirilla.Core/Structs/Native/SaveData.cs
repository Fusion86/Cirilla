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

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2PosX;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2PosY;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2SizeX;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2SizeY;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2Glossy;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp2Metallic;

        [Endian(Endianness.LittleEndian)]
        public int MakeUp2Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] MakeUp1Color;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1PosX;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1PosY;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1SizeX;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1SizeY;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1Glossy;

        [Endian(Endianness.LittleEndian)]
        public float MakeUp1Metallic;

        [Endian(Endianness.LittleEndian)]
        public int MakeUp1Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] LeftEyeColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] RightEyeColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] EyebrowColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] FacialHairColor;

        [Endian(Endianness.LittleEndian)]
        public byte EyeWidth;

        [Endian(Endianness.LittleEndian)]
        public byte EyeHeight;

        [Endian(Endianness.LittleEndian)]
        public byte SkinColorX;

        [Endian(Endianness.LittleEndian)]
        public byte SkinColorY;

        [Endian(Endianness.LittleEndian)]
        public byte Age;

        [Endian(Endianness.LittleEndian)]
        public byte Wrinkles;

        [Endian(Endianness.LittleEndian)]
        public byte NoseHeight;

        [Endian(Endianness.LittleEndian)]
        public byte MouthHeight;

        [Endian(Endianness.LittleEndian)]
        public int Gender; // 0 = male, 1 = female

        [Endian(Endianness.LittleEndian)]
        public byte BrowType;

        [Endian(Endianness.LittleEndian)]
        public byte FaceType;

        [Endian(Endianness.LittleEndian)]
        public byte EyeType;

        [Endian(Endianness.LittleEndian)]
        public byte NoseType;

        [Endian(Endianness.LittleEndian)]
        public byte MouthType;

        [Endian(Endianness.LittleEndian)]
        public byte EyebrowType;

        [Endian(Endianness.LittleEndian)]
        public byte EyelashLength; // 0 = short, 1 = average, 2 = long

        [Endian(Endianness.LittleEndian)]
        public byte FacialHairType;

        [Endian(Endianness.LittleEndian)]
        public int Unk1; // Unused?

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] HairColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] ClothingColor;

        [Endian(Endianness.LittleEndian)]
        public short HairType;

        [Endian(Endianness.LittleEndian)]
        public byte ClothingType;

        [Endian(Endianness.LittleEndian)]
        public byte Voice;

        [Endian(Endianness.LittleEndian)]
        public int Expression;
    }
}
