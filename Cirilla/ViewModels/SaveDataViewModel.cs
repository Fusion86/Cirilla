using Cirilla.Core.Models;
using System.Collections.ObjectModel;

namespace Cirilla.ViewModels
{
    class SaveDataViewModel : FileTypeTabItemViewModelBase
    {
        public SaveSlotViewModel SelectedItem { get; set; }
        public ObservableCollection<SaveSlotViewModel> Items { get; } = new ObservableCollection<SaveSlotViewModel>();

        public long SteamId => _context.Header.SteamId;

        private readonly SaveData _context;

        public SaveDataViewModel(string path) : base(path)
        {
            _context = new SaveData(path);

            foreach(var saveSlot in _context.SaveSlots)
            {
                Items.Add(new SaveSlotViewModel(saveSlot));
            }
        }

        public override void Save(string path)
        {
            _context.Save(path);
        }

        public override bool CanSave() => true;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;
    }
}
