using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ITM_Item
    {
        public int Id;

        public byte Unk3;
        public byte SubType; // Or ItemType?
        public short StorageType;

        // 0x8

        public byte Unk4;
        public byte Rarity;
        public byte CarryLimit1;
        public byte CarryLimit2;
        public short SortingOrder;
        public short Unk2; // Special Icon?

        // 0x10
        public short Unk6;
        public short Icon;
        public short Unk7;
        public byte IconColor; // See Enums/IconColor.cs
        public byte Unk8;

        // 0x18
        public int SellPrice;
        public int BuyPrice;

        // 0x20
    }
}
