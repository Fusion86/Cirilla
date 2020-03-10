using Cirilla.Core.Models;
using System.Collections.ObjectModel;

namespace Cirilla.ViewModels
{
    class EquipmentCraftingViewModel : FileTypeTabItemViewModelBase
    {
        public ObservableCollection<EquipmentCraftingEntryViewModel> Entries { get; } = new ObservableCollection<EquipmentCraftingEntryViewModel>();

        private EquipmentCrafting _context;

        public EquipmentCraftingViewModel(string path) : base(path)
        {
            _context = new EquipmentCrafting(path);

            foreach (var entry in _context.Entries)
                Entries.Add(new EquipmentCraftingEntryViewModel(entry));
        }
    }
}
