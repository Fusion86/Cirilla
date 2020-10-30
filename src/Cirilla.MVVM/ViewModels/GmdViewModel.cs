using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using CsvHelper;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Splat;
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
    public class GmdViewModel : ViewModelBase, IOpenFileViewModel
    {
        public GmdViewModel(FileInfo fileInfo, MainWindowViewModel mainWindowViewModel)
        {
            framework = Locator.Current.GetService<IFrameworkService>();
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

        public IList<FileDialogFilter> SaveFileDialogFilters { get; } = new[] { FileDialogFilter.GMD };

        private readonly IFrameworkService framework;
        private readonly GMD gmd;
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly SourceList<GmdEntryViewModel> entriesList = new SourceList<GmdEntryViewModel>();
        private static readonly Serilog.ILogger log = Log.ForContext<GmdViewModel>();

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
            var vm = new GmdEntryViewModel(entriesList.Count, new GMD_Entry { Key = key });
            entriesList.Add(vm);
        }

        private void AddPaddingEntry()
        {
            var vm = new GmdEntryViewModel(entriesList.Count, new GMD_EntryWithoutKey());
            entriesList.Add(vm);
        }

        private void ImportFromCsvHandler()
        {
            // TODO:
            throw new NotImplementedException();
        }

        private async Task ExportToCsvHandler()
        {
            string csvName = $"{Path.GetFileNameWithoutExtension(Info.Name)}";
            string? savePath = await framework.SaveFileDialog(csvName, "csv", new[] { FileDialogFilter.CSV });

            if (savePath != null)
            {
                var alert = mainWindowViewModel.ShowFlashAlert($"Exporting values to {savePath} ...", buttons: FlashMessageButtons.None);

                try
                {
                    using FileStream fs = new FileStream(savePath, FileMode.Create);
                    using TextWriter tw = new StreamWriter(fs, ExEncoding.UTF8);
                    using CsvWriter writer = new CsvWriter(tw, CultureInfo.InvariantCulture);

                    writer.Configuration.HasHeaderRecord = false;
                    writer.Configuration.Delimiter = ";";
                    writer.Configuration.ShouldQuote = (str, context) => true; // Always insert quotes

                    var records = gmd.Entries
                        .OfType<GMD_Entry>()
                        .Select(x => new StringKeyValuePair(x.Key, x.Value))
                        .ToList();

                    writer.WriteRecords(records);

                    mainWindowViewModel.ShowFlashAlert("Successfully exported values", $"Successfully exported {records.Count} values to {savePath}");
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

        private static Func<GmdEntryViewModel, bool> KeyFilterPredicate(string key)
        {
            if (string.IsNullOrEmpty(key))
                return x => true;
            return x => x.Key?.Contains(key, StringComparison.OrdinalIgnoreCase) == true;
        }

        private static Func<GmdEntryViewModel, bool> ValueFilterPredicate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return x => true;
            return x => x.Value.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
