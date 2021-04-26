// See misc/savedata_ib.bt

using Cirilla.Core.Enums;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
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
        public int Luminescent; // New in Iceborne
        public int Type;

        public static implicit operator CharacterMakeup(Compat.CharacterMakeup compat)
        {
            return new CharacterMakeup
            {
                Color = compat.Color,
                PosX = compat.PosX,
                PosY = compat.PosX,
                SizeX = compat.SizeX,
                SizeY = compat.SizeY,
                Glossy = compat.Glossy,
                Metallic = compat.Metallic,
                Type = compat.Type
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CharacterAppearance
    {
        // See CharacterAppearanceType
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Type;

        public CharacterMakeup Makeup2;
        public CharacterMakeup Makeup1;
        public CharacterMakeup Makeup3; // New in Iceborne

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
        private int Zero1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] HairColor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] ClothingColor;

        public short HairType;
        public byte ClothingType;
        public byte Voice;
        public int Expression;

        public static implicit operator CharacterAppearance(Compat.CharacterAppearance compat)
        {
            return new CharacterAppearance
            {
                Makeup2 = compat.Makeup2,
                Makeup1 = compat.Makeup1,
                LeftEyeColor = compat.LeftEyeColor,
                RightEyeColor = compat.RightEyeColor,
                EyebrowColor = compat.EyebrowColor,
                FacialHairColor = compat.FacialHairColor,
                EyeWidth = compat.EyeWidth,
                EyeHeight = compat.EyeHeight,
                SkinColorX = compat.SkinColorY,
                Age = compat.Age,
                Wrinkles = compat.Wrinkles,
                NoseHeight = compat.NoseHeight,
                MouthHeight = compat.MouthHeight,
                Gender = compat.Gender,
                BrowType = compat.BrowType,
                FaceType = compat.FaceType,
                EyeType = compat.EyeType,
                NoseType = compat.NoseType,
                MouthType = compat.MouthType,
                EyebrowType = compat.EyebrowType,
                EyelashLength = compat.EyelashLength,
                FacialHairType = compat.FacialHairType,
                HairColor = compat.HairColor,
                ClothingColor = compat.ClothingColor,
                HairType = compat.HairType,
                ClothingType = compat.ClothingType,
                Voice = compat.Voice,
                Expression = compat.Expression,
            };
        }
    }
}
