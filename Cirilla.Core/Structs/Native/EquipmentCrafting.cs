using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct EquipmentCrafting_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x0051
        public uint EntryCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EquipmentCrafting_Entry
    {
        public byte Type;
        public ushort Id;
        public ushort KeyItem;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U1)]
        public byte[] Unk1;

        public uint Rank;
        public ushort Item1_Id;
        public byte Item1_Quantity;
        public ushort Item2_Id;
        public byte Item2_Quantity;
        public ushort Item3_Id;
        public byte Item3_Quantity;
        public ushort Item4_Id;
        public byte Item4_Quantity;

        public byte Unk2;
        public byte Unk3;
        public byte Unk4;
        public byte Unk5;
    }
}
