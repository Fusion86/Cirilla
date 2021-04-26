using Cirilla.Core.Helpers;

namespace Cirilla.Core.Enums
{
    public enum Gender : int
    {
        Male = 0,
        Female = 1
    }

    public enum EyelashLength : byte
    {
        Short = 0,
        Average = 1,
        Long = 2
    }

    public enum PalicoVoiceType : byte
    {
        Type1 = 0,
        Type2 = 1,
        Type3 = 2
    }

    public enum PalicoVoicePitch : byte
    {
        MediumPitch = 0,
        LowPitch = 1,
        HighPitch = 2
    }

    public class CharacterAppearanceType : Enumeration
    {
        // I don't know for sure, see notes inside the appearance.bt file
        public static readonly CharacterAppearanceType Zero = new CharacterAppearanceType(new byte[] { 0, 0, 0, 0 });
        public static readonly CharacterAppearanceType BaseGamePlayer = new CharacterAppearanceType(new byte[] { 0x02, 0x00, 0x00, 0x00 });
        public static readonly CharacterAppearanceType Preset = new CharacterAppearanceType(new byte[] { 0x09, 0x18, 0x09, 0x00 });

        public byte[] Value { get; }

        private CharacterAppearanceType(byte[] b)
        {
            Value = b;
        }
    }
}
