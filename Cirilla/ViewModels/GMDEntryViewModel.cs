using Cirilla.Core.Models;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class GMDEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Index { get; set; }
        public IGMD_Entry Entry { get; }

        public GMDEntryViewModel()
        {
            Entry = new GMD_Entry { Key = "CIRILLA_KEY", Value = "CIRILLA_VALUE" };
        }

        public GMDEntryViewModel(int index, IGMD_Entry entry)
        {
            Index = index;
            Entry = entry;
        }

        public string Key
        {
            get => (Entry as GMD_Entry)?.Key ?? ""; // Idk if beautiful or disgusting

            set
            {
                GMD_Entry entry = (Entry as GMD_Entry);

                if (entry != null)
                    entry.Key = value;
            }
        }

        public string Value { get => Entry.Value; set => Entry.Value = value; }
    }
}
