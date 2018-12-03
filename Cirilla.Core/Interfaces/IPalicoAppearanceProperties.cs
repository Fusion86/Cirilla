using System.Drawing;

namespace Cirilla.Core.Interfaces
{
    public interface IPalicoAppearanceProperties
    {
        Color PatternColor1 { get; set; }
        Color PatternColor2 { get; set; }
        Color PatternColor3 { get; set; }
        Color FurColor { get; set; }
        Color LeftEyeColor { get; set; }
        Color RightEyeColor { get; set; }
        Color ClothingColor { get; set; }

        float FurLength { get; set; }
        float FurThickness { get; set; }
        byte PatternType { get; set; }
        byte EyeType { get; set; }
        byte EarType { get; set; }
        byte TailType { get; set; }
        ushort VoiceType { get; set; }
        ushort VoicePitch { get; set; }
    }
}
