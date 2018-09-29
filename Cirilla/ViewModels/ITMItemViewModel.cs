using Cirilla.Core.Structs.Native;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class ITMItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ITM_Item _item;

        public ITMItemViewModel(ITM_Item item)
        {
            _item = item;
        }

        public int Id
        {
            get => _item.Id;
            set => _item.Id = value;
        }

        public int BuyPrice
        {
            get => _item.BuyPrice;
            set => _item.BuyPrice = value;
        }

        public int SellPrice
        {
            get => _item.SellPrice;
            set => _item.SellPrice = value;
        }

        public short SortingOrder
        {
            get => _item.SortingOrder;
            set => _item.SortingOrder = value;
        }

        public byte Rarity
        {
            get => _item.Rarity;
            set => _item.Rarity = value;
        }

        public byte SubType
        {
            get => _item.SubType;
            set => _item.SubType = value;
        }

        public short StorageType
        {
            get => _item.StorageType;
            set => _item.StorageType = value;
        }
    }
}
