using Cirilla.Core.Enums;
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
        byte OutlineType { get; set; }
        byte PupilType { get; set; }
        PalicoVoiceType VoiceType { get; set; }
        PalicoVoicePitch VoicePitch { get; set; }
    }
}
