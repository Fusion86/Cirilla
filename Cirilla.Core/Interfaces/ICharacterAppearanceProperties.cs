using Cirilla.Core.Enums;
using System.Drawing;

namespace Cirilla.Core.Interfaces
{
    public interface ICharacterAppearanceProperties
    {
        Color Makeup2Color { get; set; }
        float Makeup2PosX { get; set; }
        float Makeup2PosY { get; set; }
        float Makeup2SizeX { get; set; }
        float Makeup2SizeY { get; set; }
        float Makeup2Glossy { get; set; }
        float Makeup2Metallic { get; set; }
        int Makeup2Type { get; set; }

        Color Makeup1Color { get; set; }
        float Makeup1PosX { get; set; }
        float Makeup1PosY { get; set; }
        float Makeup1SizeX { get; set; }
        float Makeup1SizeY { get; set; }
        float Makeup1Glossy { get; set; }
        float Makeup1Metallic { get; set; }
        int Makeup1Type { get; set; }

        Color LeftEyeColor { get; set; }
        Color RightEyeColor { get; set; }
        Color EyebrowColor { get; set; }
        Color FacialHairColor { get; set; }

        byte EyeWidth { get; set; }
        byte EyeHeight { get; set; }
        byte SkinColorX { get; set; }
        byte SkinColorY { get; set; }
        byte Age { get; set; }
        byte Wrinkles { get; set; }
        byte NoseHeight { get; set; }
        byte MouthHeight { get; set; }

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
    }
}
