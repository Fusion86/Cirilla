using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ArmorData_Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U1)]
        public byte[] Magic; // 0x005D
        public uint EntryCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArmorData_Entry
    {
        public ushort Index;
        public int Unk1;
        public byte Variant;
        public byte SetId;
        public short Unk2;
        public byte EquipSlot;
        public ushort Defense;
        public ushort Texmod1Id;
        public ushort Texmod2Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding3;

        public byte Rarity;
        public uint Cost;
        public byte FireResistance;
        public byte WaterResistance;
        public byte IceResistance;
        public byte ThunderResistance;
        public byte DragonResistance;
        public byte GemSlotCount;
        public byte GemSlot1Level;
        public byte GemSlot2Level;
        public byte GemSlot3Level;
        public ushort SetSkill1Id;
        public byte SetSkill1Level;
        public ushort SetSkill2Id;
        public byte SetSkill2Level;
        public ushort Skill1Id;
        public byte Skill1Level;
        public ushort Skill2Id;
        public byte Skill2Level;
        public ushort Skill3Id;
        public byte Skill3Level;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding5;

        public ushort StringIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] Padding6;
    }
}
