using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    public struct CharacterMakeup
    {
        private int Unk;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Color;

        public float PosX;
        public float PosY;
        public float SizeX;
        public float SizeY;
        public float Glossy;
        public float Metallic;
        public int Type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CharacterAppearance
    {
        CharacterMakeup Makeup2;
        CharacterMakeup Makeup1;

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
