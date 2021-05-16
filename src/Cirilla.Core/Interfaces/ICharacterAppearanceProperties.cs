using Cirilla.Core.Enums;
using System.Drawing;

namespace Cirilla.Core.Interfaces
{
    // It is not possible to use this.
    // TL;DR We need to store a ref struct in a field, which is not possible (atm).
    //public interface ICharacterMakeup
    //{
    //    Color Color { get; set; }
    //    float PosX { get; set; }
    //    float PosY { get; set; }
    //    float SizeX { get; set; }
    //    float SizeY { get; set; }
    //    float Glossy { get; set; }
    //    float Metallic { get; set; }
    //    int Type { get; set; }
    //}

    public interface ICharacterAppearanceProperties
    {
        Color Makeup2Color { get; set; }
        float Makeup2PosX { get; set; }
        float Makeup2PosY { get; set; }
        float Makeup2SizeX { get; set; }
        float Makeup2SizeY { get; set; }
        float Makeup2Glossy { get; set; }
        float Makeup2Metallic { get; set; }
        float Makeup2Luminescent { get; set; }
        int Makeup2Type { get; set; }

        Color Makeup1Color { get; set; }
        float Makeup1PosX { get; set; }
        float Makeup1PosY { get; set; }
        float Makeup1SizeX { get; set; }
        float Makeup1SizeY { get; set; }
        float Makeup1Glossy { get; set; }
        float Makeup1Metallic { get; set; }
        float Makeup1Luminescent { get; set; }
        int Makeup1Type { get; set; }

        Color Makeup3Color { get; set; }
        float Makeup3PosX { get; set; }
        float Makeup3PosY { get; set; }
        float Makeup3SizeX { get; set; }
        float Makeup3SizeY { get; set; }
        float Makeup3Glossy { get; set; }
        float Makeup3Metallic { get; set; }
        float Makeup3Luminescent { get; set; }
        int Makeup3Type { get; set; }

        Color LeftEyeColor { get; set; }
        Color RightEyeColor { get; set; }
        Color EyebrowColor { get; set; }
        Color FacialHairColor { get; set; }

        sbyte EyeWidth { get; set; }
        sbyte EyeHeight { get; set; }
        byte SkinColorX { get; set; }
        byte SkinColorY { get; set; }
        byte Age { get; set; }
        byte Wrinkles { get; set; }
        sbyte NoseHeight { get; set; }
        sbyte MouthHeight { get; set; }

        Gender Gender { get; set; }

        byte BrowType { get; set; }
        byte FaceType { get; set; }
        byte EyeType { get; set; }
        byte NoseType { get; set; }
        byte MouthType { get; set; }
        byte EyebrowType { get; set; }
        EyelashLength EyelashLength { get; set; }
        byte FacialHairType { get; set; }

        Color HairColor { get; set; }
        Color ClothingColor { get; set; }

        short HairType { get; set; }
        byte ClothingType { get; set; }
        byte Voice { get; set; }
        int Expression { get; set; }

        byte[] Type { get; set; }
    }
}
