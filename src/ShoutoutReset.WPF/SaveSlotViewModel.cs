using Cirilla.Core.Models;
using System.Collections.ObjectModel;

namespace ShoutoutReset.WPF
{
    public class SaveSlotViewModel : ViewModelBase
    {
        public string DisplayName => $"{saveSlot.HunterName} (HR {saveSlot.HunterRank})";

        public ObservableCollection<ShoutoutViewModel> Shoutouts { get; } = new();

        private readonly SaveSlot saveSlot;

        public SaveSlotViewModel(SaveSlot saveSlot)
        {
            this.saveSlot = saveSlot;

            for (int i = 0; i < saveSlot.Native.Shoutouts.Length; i++)
            {
                Shoutouts.Add(new ShoutoutViewModel(saveSlot, i));
            }
        }
    }
}
