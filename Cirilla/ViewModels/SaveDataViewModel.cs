using Cirilla.Core.Models;
using System;
using System.Collections.Generic;

namespace Cirilla.ViewModels
{
    class SaveDataViewModel : FileTypeTabItemViewModelBase
    {
        public SaveSlot SelectedItem { get; set; }
        public IReadOnlyList<SaveSlot> Items => _context.SaveSlots;

        public long SteamId => _context.Header.SteamId;

        private SaveData _context;

        public SaveDataViewModel(string path) : base(path)
        {
            _context = new SaveData(path);
        }

        public override void Save(string path)
        {
            throw new NotImplementedException();
        }

        public override bool CanSave() => false;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;
    }
}
