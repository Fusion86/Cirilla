using Cirilla.Core.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace Cirilla.ViewModels
{
    class SaveDataViewModel : FileTypeTabItemViewModelBase
    {
        public SaveSlotViewModel SelectedItem { get; set; }
        public ObservableCollection<SaveSlotViewModel> Items { get; } = new ObservableCollection<SaveSlotViewModel>();

        public long SteamId => _context.Header.SteamId;

        private SaveData _context;

        public SaveDataViewModel(string path) : base(path)
        {
            _context = new SaveData(path);

            foreach (var saveSlot in _context.SaveSlots)
            {
                Items.Add(new SaveSlotViewModel(saveSlot));
            }
        }

        public override void Save()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(Filepath);
            sfd.Filter = "Encrypted SAVEDATA1000|*|Decrypted SAVEDATA1000|*";

            if (sfd.ShowDialog() == true)
            {
                bool save_encrypted = sfd.FilterIndex == 1; // index 2 = unencrypted
                _context.Save(sfd.FileName, save_encrypted);
            }
        }

        public override bool CanSave() => true;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;
    }
}
