using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdViewModel : ViewModelBase, IExplorerFileItem
    {
        public GmdViewModel(FileInfo fileInfo, MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            Info = fileInfo;
            gmd = new GMD(fileInfo.FullName);

            for (int i = 0; i < gmd.Entries.Count; i++)
                entriesList.Add(new GmdEntryViewModel(i, gmd.Entries[i]));

            AddEntryCommand = ReactiveCommand.Create(AddEntry);
            AddPaddingEntryCommand = ReactiveCommand.Create(AddPaddingEntry);
            ImportFromCsvCommand = ReactiveCommand.Create(ImportFromCsvHandler);
            ExportToCsvCommand = ReactiveCommand.CreateFromTask(ExportToCsvHandler);
            DeleteRowsCommand = ReactiveCommand.Create<IList<GmdEntryViewModel>>(DeleteRows);

            var keyFilter = this.WhenValueChanged(t => t.KeySearchQuery)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(KeyFilterPredicate);

            var valueFilter = this.WhenValueChanged(t => t.ValueSearchQuery)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(ValueFilterPredicate);

            entriesList.Connect()
                .Filter(keyFilter)
                .Filter(valueFilter)
                .Sort(SortExpressionComparer<GmdEntryViewModel>.Ascending(x => x.Index))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out filteredEntriesBinding)
                .Subscribe();
        }

        public ReactiveCommand<Unit, Unit> AddEntryCommand { get; }
        public ReactiveCommand<Unit, Unit> AddPaddingEntryCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportFromCsvCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportToCsvCommand { get; }
        public ReactiveCommand<IList<GmdEntryViewModel>, Unit> DeleteRowsCommand { get; }

        [Reactive] public string KeySearchQuery { get; set; } = "";
        [Reactive] public string ValueSearchQuery { get; set; } = "";
        [Reactive] public GmdEntryViewModel? SelectedEntry { get; set; }

        private readonly ReadOnlyObservableCollection<GmdEntryViewModel> filteredEntriesBinding;
        public ReadOnlyObservableCollection<GmdEntryViewModel> FilteredEntries => filteredEntriesBinding;

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;
        public string StatusText => Info.FullName;

        public IList<FileDialogFilter> SaveFileDialogFilters { get; } = new[] { FileDialogFilter.GMD };

        private readonly GMD gmd;
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly SourceList<GmdEntryViewModel> entriesList = new();
        private static readonly ILogger log = Log.ForContext<GmdViewModel>();

        public bool Close()
        {
            return true;
        }

        public async Task Save(string path)
        {
            await Task.Run(() => gmd.Save(path));
        }

        public IObservable<IChangeSet<GmdEntryViewModel>> Connect()
        {
            return entriesList.Connect();
        }

        private void AddEntry()
        {
            string key = $"CIRILLA_KEY_" + entriesList.Count;
            var entry = new GMD_Entry { Key = key };
            gmd.Entries.Add(entry);

            var vm = new GmdEntryViewModel(entriesList.Count, entry);
            entriesList.Add(vm);
        }

        private void AddPaddingEntry()
        {
            var entry = new GMD_EntryWithoutKey();
            gmd.Entries.Add(entry);

            var vm = new GmdEntryViewModel(entriesList.Count, entry);
            entriesList.Add(vm);
        }

        private void ImportFromCsvHandler()
        {
            // TODO:
        }

        private async Task ExportToCsvHandler()
        {
            string csvName = $"{Path.GetFileNameWithoutExtension(Info.Name)}";
            string? savePath = await mainWindowViewModel.Framework.SaveFileDialog(csvName, "csv", new[] { FileDialogFilter.CSV });

            if (savePath != null)
            {
                var alert = mainWindowViewModel.ShowFlashAlert($"Exporting values to {savePath} ...", buttons: FlashMessageButtons.None);

                try
                {
                    var count = gmd.ExportToCsv(savePath);
                    mainWindowViewModel.ShowFlashAlert("Successfully exported values", $"Successfully exported {count} values to {savePath}");
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Could not export values.");
                    mainWindowViewModel.ShowFlashAlert("Could not export values!", ex.Message, FlashMessageButtons.Ok);
                }

                alert.Close();
            }
        }

        private void DeleteRows(IList<GmdEntryViewModel> rowsToDelete)
        {
            entriesList.Edit(x =>
            {
                foreach (var row in rowsToDelete)
                {
                    if (!x.Remove(row) || !gmd.Entries.Remove(row.entry))
                        log.Error($"Could not remove row '{row.Key}'.");
                }
            });
        }

        private static Func<GmdEntryViewModel, bool> KeyFilterPredicate(string? key)
        {
            if (string.IsNullOrEmpty(key))
                return x => true;
            return x => x.Key?.Contains(key, StringComparison.OrdinalIgnoreCase) == true;
        }

        private static Func<GmdEntryViewModel, bool> ValueFilterPredicate(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return x => true;
            return x => x.Value.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
