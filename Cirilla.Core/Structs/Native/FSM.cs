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
        public byte[] Magic; // "GMD"

        public byte Padding1;
        
        public short Version;
        public short Type;

        [Endian(Endianness.LittleEndian)]
        public int Unk3;

        [Endian(Endianness.LittleEndian)]
        public int Unk4;

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public int StructCount;

        [Endian(Endianness.LittleEndian)]
        public int OffsetToData;

        // 0x18

        #endregion

        public string MagicString => Encoding.ASCII.GetString(Magic);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FSM_StructHeader
    {
        [Endian(Endianness.LittleEndian)]
        public int Unk1; // Offset containing struct data

        [Endian(Endianness.LittleEndian)]
        public int Hash; // Unique identifier for struct

        [Endian(Endianness.LittleEndian)]
        public int VariableCount;

        [Endian(Endianness.LittleEndian)]
        public int Unk4;

        // 0x10
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct FSM_VariableEntry
    {
        [Endian(Endianness.LittleEndian)]
        public long NameOffset;

        public byte Type;
        public byte Byte1;

        [Endian(Endianness.LittleEndian)]
        public long Size; // Variable size

        [Endian(Endianness.LittleEndian)]
        public long Int1;

        [Endian(Endianness.LittleEndian)]
        public long Int2;

        [Endian(Endianness.LittleEndian)]
        public long Int3;

        [Endian(Endianness.LittleEndian)]
        public short Short1;

        [Endian(Endianness.LittleEndian)]
        public long Int4;

        [Endian(Endianness.LittleEndian)]
        public long Int5;

        [Endian(Endianness.LittleEndian)]
        public long Int6;

        [Endian(Endianness.LittleEndian)]
        public long Int7;

        [Endian(Endianness.LittleEndian)]
        public int Int8;
    }
}
