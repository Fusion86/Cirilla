using Cirilla.Core.Models;

namespace Cirilla.Avalonia.ViewModels
{
    public class GmdEntryViewModel
    {
        public GmdEntryViewModel(int index, IGMD_Entry entry)
        {
            Index = index;
            this.entry = entry;
        }

        public int Index { get; set; }
        private readonly IGMD_Entry entry;

        public string? Key
        {
            get => (entry as GMD_Entry)?.Key;

            set
            {
                if (entry is GMD_Entry x)
                    x.Key = value;
            }
        }

        public string Value { get => entry.Value; set => entry.Value = value; }
    }
}
