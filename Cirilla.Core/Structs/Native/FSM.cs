using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;
using System.Text;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_Header
    {
        #region Native

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // "XFS"

        public byte Padding1;

        public short Version;
        public short Type;
        public int Unk3;
        public int Unk4;

        // 0x10
        public int StructCount;
        public int OffsetToData;

        // 0x18

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_StructHeader
    {
        public int Hash;
        public int Unk1; // Zero?
        public int VariableCount;
        public int Unk2; // Zero?

        // 0x10
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FSM_VariableEntry
    {
        public long NameOffset;
        public byte Type;
        public byte Byte1;
        public long Size; // Variable size
        public long Int1;
        public long Int2;
        public long Int3;
        public short Short1;
        public long Int4;
        public long Int5;
        public long Int6;
        public long Int7;
        public int Int8;
    }
}
