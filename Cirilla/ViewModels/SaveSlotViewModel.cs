using Cirilla.Core.Models;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    class SaveSlotViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppearanceViewModel Appearance { get; }

        private SaveSlot _saveSlot;

        public SaveSlotViewModel(SaveSlot saveSlot)
        {
            _saveSlot = saveSlot;

            Appearance = new AppearanceViewModel(_saveSlot);
        }

        public string HunterName { get => _saveSlot.HunterName; set => _saveSlot.HunterName = value; }
        public int HunterRank { get => _saveSlot.HunterRank; set => _saveSlot.HunterRank = value; }
        public int Zeni { get => _saveSlot.Zeni; set => _saveSlot.Zeni = value; }
        public int ResearchPoints { get => _saveSlot.ResearchPoints; set => _saveSlot.ResearchPoints = value; }
        public int HunterXp { get => _saveSlot.HunterXp; set => _saveSlot.HunterXp = value; }
    }
}
