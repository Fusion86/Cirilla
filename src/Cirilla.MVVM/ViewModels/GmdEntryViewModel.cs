using Cirilla.Core.Models;
using Serilog;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdEntryViewModel
    {
        public GmdEntryViewModel(int index, IGMD_Entry entry)
        {
            Index = index;
            this.entry = entry;
        }

        public int Index { get; set; }
        internal readonly IGMD_Entry entry;
        private static readonly ILogger log = Log.ForContext<GmdEntryViewModel>();

        public string? Key
        {
            get => (entry as GMD_Entry)?.Key;

            set
            {
                if (entry is GMD_Entry x)
                    x.Key = value;
                else
                    log.Error("Can't set 'Key' for entry of type 'GMD_EntryWithoutKey'.");
            }
        }

        public string Value { get => entry.Value; set => entry.Value = value; }
    }
}
