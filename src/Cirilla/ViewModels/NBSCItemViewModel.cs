using Cirilla.Core.Structs.Native;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class NBSCItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NBSC_NPC Item => _item;

        private NBSC_NPC _item;
        private NBSCViewModel _parent;

        public NBSCItemViewModel(NBSC_NPC item, NBSCViewModel parent)
        {
            _item = item;
            _parent = parent;
        }

        public int Id
        {
            get => _item.Id;
            set => _item.Id = value;
        }

        public string Name => _parent.GetNameForItem(this);

        public float Scale
        {
            get => _item.Scale;
            set => _item.Scale = value;
        }
    }
}
