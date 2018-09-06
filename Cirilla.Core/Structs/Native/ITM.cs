using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ITM_Item
    {
        [Endian(Endianness.LittleEndian)]
        public int Id;

        public byte Unk3;
        public byte SubType; // Or ItemType?

        [Endian(Endianness.LittleEndian)]
        public short StorageType;

        // 0x8

        public byte Unk4;
        public byte Rarity;

        public byte CarryLimit1;
        public byte CarryLimit2;

        [Endian(Endianness.LittleEndian)]
        public short SortingOrder;

        [Endian(Endianness.LittleEndian)]
        public short Unk2; // Special Icon?

        // 0x10

        [Endian(Endianness.LittleEndian)]
        public short Unk6;

        [Endian(Endianness.LittleEndian)]
        public short Icon;

        [Endian(Endianness.LittleEndian)]
        public short Unk7;

        public byte IconColor; // See Enums/IconColor.cs
        public byte Unk8;

        // 0x18

        [Endian(Endianness.LittleEndian)]
        public int SellPrice;

        [Endian(Endianness.LittleEndian)]
        public int BuyPrice;

        // 0x20
    }
}
