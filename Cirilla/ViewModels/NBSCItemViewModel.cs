using Cirilla.Core.Structs.Native;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class NBSCItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NBSC_NPC Item => _item;

        private NBSC_NPC _item;

        public NBSCItemViewModel(NBSC_NPC item)
        {
            _item = item;
        }

        public int Id
        {
            get => _item.Id;

            set
            {
                _item.Id = value;
            }
        }

        public float Scale
        {
            get => _item.Scale;

            set
            {
                _item.Scale = value;
            }
        }
    }
}
