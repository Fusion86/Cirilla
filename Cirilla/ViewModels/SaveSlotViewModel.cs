using Cirilla.Core.Models;
using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class SaveSlotViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppearanceViewModel Appearance { get; }

        private readonly SaveSlot _context;

        public SaveSlotViewModel(SaveSlot context)
        {
            _context = context;

            Appearance = new AppearanceViewModel(_context);
        }

        public string HunterName { get => _context.HunterName; set => _context.HunterName = value; }
        public int HunterRank { get => _context.HunterRank; set => _context.HunterRank = value; }
        public int Zeni { get => _context.Zeni; set => _context.Zeni = value; }
        public int ResearchPoints { get => _context.ResearchPoints; set => _context.ResearchPoints = value; }
        public int HunterXp { get => _context.HunterXp; set => _context.HunterXp = value; }

        public string GetAppearanceJson() => _context.GetAppearanceJson();
    }
}
