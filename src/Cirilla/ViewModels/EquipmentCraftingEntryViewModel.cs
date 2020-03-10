using Cirilla.Core.Structs.Native;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    class EquipmentCraftingEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private EquipmentCrafting_Entry _entry;

        public EquipmentCraftingEntryViewModel(EquipmentCrafting_Entry entry)
        {
            _entry = entry;
        }

        public byte Type
        {
            get => _entry.Type;
            set => _entry.Type = value;
        }

        public ushort Id
        {
            get => _entry.Id;
            set => _entry.Id = value;
        }

        public ushort KeyItem
        {
            get => _entry.KeyItem;
            set => _entry.KeyItem = value;
        }

        public uint Rank
        {
            get => _entry.Rank;
            set => _entry.Rank = value;
        }

        public ushort Item1_Id
        {
            get => _entry.Item1_Id;
            set => _entry.Item1_Id = value;
        }

        public byte Item1_Quantity
        {
            get => _entry.Item1_Quantity;
            set => _entry.Item1_Quantity = value;
        }

        public ushort Item2_Id
        {
            get => _entry.Item2_Id;
            set => _entry.Item2_Id = value;
        }

        public byte Item2_Quantity
        {
            get => _entry.Item2_Quantity;
            set => _entry.Item2_Quantity = value;
        }

        public ushort Item3_Id
        {
            get => _entry.Item3_Id;
            set => _entry.Item3_Id = value;
        }

        public byte Item3_Quantity
        {
            get => _entry.Item3_Quantity;
            set => _entry.Item3_Quantity = value;
        }

        public ushort Item4_Id
        {
            get => _entry.Item4_Id;
            set => _entry.Item4_Id = value;
        }

        public byte Item4_Quantity
        {
            get => _entry.Item4_Quantity;
            set => _entry.Item4_Quantity = value;
        }
    }
}
