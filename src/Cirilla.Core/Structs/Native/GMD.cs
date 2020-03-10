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
        public uint Hash1;
        public uint Hash2;
        public short Zero1;
        public short Unk1; // Flag?

        // 0x10
        public int KeyOffset;
        public int Zero2;
        public int Zero3;
        public int Zero4;

        // 0x20
    }
}
