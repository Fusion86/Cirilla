using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GMD_Header
    {
        #region Native

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "GMD"

        public byte Padding1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Version; // Version?
        public int Language;
        public int Unk1; // Zero

        // 0x10
        public int Unk2; // Zero
        public int KeyCount;
        public int StringCount; // Usually the same as KeyCount
        public int KeyBlockSize;

        // 0x20
        public int StringBlockSize;
        public int FilenameLength;

        // 0x28

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GMD_InfoTableEntry
    {
        public int StringIndex;
        public int Unk2;
        public int Unk3;
        public short Short1;
        public short Short2;

        // 0x10
        public int KeyOffset;
        public int Unk6;
        public int Unk7;
        public int Unk8;

        // 0x20
    }
}
