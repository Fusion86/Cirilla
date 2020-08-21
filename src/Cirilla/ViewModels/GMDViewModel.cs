using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using Cirilla.Models;
using Cirilla.Windows;
using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cirilla.ViewModels
{
    public class GMDViewModel : FileTypeTabItemViewModelBase
    {
        public ObservableCollection<KeyValueViewModel> HeaderMetadata { get; } = new ObservableCollection<KeyValueViewModel>();
        public ObservableCollection<GMDEntryViewModel> Entries { get; } = new ObservableCollection<GMDEntryViewModel>();

        public string Name => _context.Filename;

        public string SearchQuery { get; set; }
        public ICollectionView FilteredEntries { get; }

        public RelayCommand AddEntryCommand { get; }
        public RelayCommand AddEntryNoKeyCommand { get; }
        public RelayCommand TriggerSearchCommand { get; }
        public RelayCommand ImportCsvCommand { get; }
        public RelayCommand ExportCsvCommand { get; }

        private GMD _context;

        public GMDViewModel(string path) : base(path)
        {
            _context = new GMD(path);

            // Commands
            AddEntryCommand = new RelayCommand(AddEntry, IsUnsafeModeEnabled);
            AddEntryNoKeyCommand = new RelayCommand(AddEntryNoKey, IsUnsafeModeEnabled);
            TriggerSearchCommand = new RelayCommand(TriggerSearch);
            ImportCsvCommand = new RelayCommand(ImportCsv, CanImportCsv);
            ExportCsvCommand = new RelayCommand(ExportCsv, CanExportCsv);

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

            // Filtered Entries CollectionViewSource
            FilteredEntries = CollectionViewSource.GetDefaultView(Entries);
            FilteredEntries.Filter = Entries_Filter;
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

        public bool IsUnsafeModeEnabled()
        {
            if (Properties.Settings.Default.Config != null)
                return Properties.Settings.Default.Config.UnsafeModeEnabled;

            return false;
        }

        public void TriggerSearch()
        {
            FilteredEntries.Refresh();
            NotifyPropertyChanged(nameof(FilteredEntries));
        }

        public bool CanImportCsv() => true;
        public void ImportCsv()
        {
            new ImportCsvWindow(this).ShowDialog();
        }

        public bool CanExportCsv() => true;
        public void ExportCsv()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV UTF-8 (Comma delimited)|*.csv";
            sfd.FileName = Name + ".csv";

            if (sfd.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                using (TextWriter tw = new StreamWriter(fs, ExEncoding.UTF8))
                using (CsvWriter writer = new CsvWriter(tw, CultureInfo.InvariantCulture))
                {
                    writer.Configuration.Delimiter = ";";

                    foreach (var entry in _context.Entries.OfType<GMD_Entry>())
                    {
                        writer.WriteRecord(new StringKeyValuePair(entry.Key, entry.Value));
                        writer.NextRecord();
                    }
                }
            }
        }

        private bool Entries_Filter(object obj)
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return true;

            GMDEntryViewModel entry = obj as GMDEntryViewModel;
            return entry.Value.IndexOf(SearchQuery, StringComparison.InvariantCultureIgnoreCase) != -1;
        }
    }
}
