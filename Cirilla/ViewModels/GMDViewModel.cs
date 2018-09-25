using Cirilla.Core.Models;
using Cirilla.Core.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cirilla.ViewModels
{
    public class GMDViewModel : FileTypeTabItemViewModelBase
    {
        public ObservableCollection<KeyValueViewModel> HeaderMetadata { get; } = new ObservableCollection<KeyValueViewModel>();
        public ObservableCollection<GMDEntryViewModel> Entries { get; } = new ObservableCollection<GMDEntryViewModel>();

        public RelayCommand AddEntryCommand { get; }
        public RelayCommand AddEntryNoKeyCommand { get; }

        private GMD _context;

        public GMDViewModel(string path) : base(path)
        {
            _context = new GMD(path);

            // Commands
            AddEntryCommand = new RelayCommand(AddEntry, IsUnsafeModeEnabled);
            AddEntryNoKeyCommand = new RelayCommand(AddEntryNoKey, IsUnsafeModeEnabled);

            // Header metadata
            HeaderMetadata.Add(new KeyValueViewModel("Version", "0x" + _context.Header.Version.ToHexString()));
            HeaderMetadata.Add(new KeyValueViewModel("Language", _context.Header.Language.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("KeyCount", _context.Header.KeyCount.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("StringCount", _context.Header.StringCount.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("KeyBlockSize", _context.Header.KeyBlockSize.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("StringBlockSize", _context.Header.StringBlockSize.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("FilenameLength", _context.Header.FilenameLength.ToString()));
            HeaderMetadata.Add(new KeyValueViewModel("Filename", _context.Filename));

            // Entries
            for (int i = 0; i < _context.Entries.Count; i++)
                    Entries.Add(new GMDEntryViewModel(i, _context.Entries[i]));
        }

        public override void Save(string path)
        {
            // Replace all GMD entries with the new/edited ones
            _context.Entries.Clear();

            foreach (GMDEntryViewModel vm in Entries)
                _context.Entries.Add(vm.Entry);

            try
            {
                _context.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public override bool CanSave() => true;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;

        public void AddEntry()
        {
            string key = $"CIRILLA_KEY_" + Entries.Count;
            GMDEntryViewModel vm = new GMDEntryViewModel(Entries.Count, new GMD_Entry { Key = key });
            Entries.Add(vm);
        }

        public void AddEntryNoKey()
        {
            GMDEntryViewModel vm = new GMDEntryViewModel(Entries.Count, new GMD_EntryWithoutKey());
            Entries.Add(vm);
        }

        public bool IsUnsafeModeEnabled() => Properties.Settings.Default.Config.UnsafeModeEnabled;
    }
}
