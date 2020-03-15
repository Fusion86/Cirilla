using Cirilla.Core.Enums;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native.Compat
{
    public struct CharacterMakeup
    {
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
        public CharacterMakeup Makeup2;
        public CharacterMakeup Makeup1;

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
        public Gender Gender;
        public byte BrowType;
        public byte FaceType;
        public byte EyeType;
        public byte NoseType;
        public byte MouthType;
        public byte EyebrowType;
        public EyelashLength EyelashLength;
        public byte FacialHairType;
        public int Zero1;

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
