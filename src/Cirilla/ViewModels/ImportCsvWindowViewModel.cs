using Cirilla.Core.Helpers;
using Cirilla.Models;
using CsvHelper;
using Microsoft.Win32;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Cirilla.ViewModels
{
    public class ImportCsvWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GMDViewModel GMD { get; }

        public ObservableCollection<ObjectValuesDiffViewModel> Entries { get; } = new ObservableCollection<ObjectValuesDiffViewModel>();
        public ICollectionView FilteredEntries { get; }

        #region Commands

        public RelayCommand OpenCsvCommand { get; }
        public RelayCommand ApplyCommand { get; }

        #endregion

        private Window _window;

        public ImportCsvWindowViewModel(GMDViewModel gmd, Window window)
        {
            GMD = gmd;
            _window = window;

            OpenCsvCommand = new RelayCommand(OpenCsv);
            ApplyCommand = new RelayCommand(ApplyChanges, CanApplyChanges);

            FilteredEntries = CollectionViewSource.GetDefaultView(Entries);
            FilteredEntries.Filter = Entries_Filter;
        }

        private bool _hideUnchangedEntries;
        public bool HideUnchangedEntries
        {
            get => _hideUnchangedEntries;

            set
            {
                _hideUnchangedEntries = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HideUnchangedEntries)));
                FilteredEntries.Refresh();
            }
        }

        public void OpenCsv()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV UTF-8 (Comma delimited)|*.csv";
            ofd.FileName = GMD.Name + ".csv";

            if (ofd.ShowDialog() == true)
            {
                List<StringKeyValuePair> values;

                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (TextReader tr = new StreamReader(fs, ExEncoding.UTF8))
                using (CsvReader csv = new CsvReader(tr, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Configuration.Delimiter = ";";

                    values = csv.GetRecords<StringKeyValuePair>().ToList();
                }

                Entries.Clear();

                foreach (var newEntry in values)
                {
                    GMDEntryViewModel entry;

                    // If key matches, add
                    if ((entry = GMD.Entries.FirstOrDefault(x => x.Key == newEntry.Key)) != null)
                    {
                        Entries.Add(new ObjectValuesDiffViewModel(entry.Key, entry.Value, newEntry.Value));
                    }
                }
            }
        }

        public bool CanApplyChanges() => Entries.Count > 0;
        public void ApplyChanges()
        {
            int updatedEntriesCount = 0;

            foreach (var newEntry in Entries)
            {
                // Ignore values that are unchanged
                if (!newEntry.HasChanges)
                    continue;

                var itemToUpdate = GMD.Entries.FirstOrDefault(x => x.Key == newEntry.Name);

                if (itemToUpdate == null)
                {
                    // Should never happen
                    Log.Warning($"newEntry with key '{newEntry.Name}' doesn't exist in the GMD!");
                }
                else
                {
                    // Always is a string, so the cast is fine
                    itemToUpdate.Value = (string)newEntry.NewValue;
                    updatedEntriesCount++;
                }
            }

            MessageBox.Show($"Updated {updatedEntriesCount} entries", "Update complete");
            Log.Information($"Imported {updatedEntriesCount} changes from CSV file");
            _window.Close();
        }

        private bool Entries_Filter(object obj)
        {
            if (!HideUnchangedEntries)
                return true;

            ObjectValuesDiffViewModel diff = (ObjectValuesDiffViewModel)obj;
            return (string)diff.NewValue != (string)diff.CurrentValue;
        }
    }
}
