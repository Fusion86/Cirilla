using Cirilla.Core.Helpers;
using Cirilla.MVVM.Common;
using CsvHelper;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdCsvViewModel : ViewModelBase, IOpenFileViewModel
    {
        public GmdCsvViewModel(FileInfo fileInfo, MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            var canPopulateImportEntries = this.WhenAnyValue(x => x.SelectedGmdFile, (GmdViewModel? x) => x != null);

            Info = fileInfo;
            ReloadCsvCommand = ReactiveCommand.Create(() => LoadCsv(Info.FullName));
            ApplyAndSaveCommand = ReactiveCommand.CreateFromTask(ApplyAndSave);
            PopulateImportEntriesCommand = ReactiveCommand.Create(PopulateImportEntries, canPopulateImportEntries);
            PickGmdFileToLinkCommand = ReactiveCommand.CreateFromTask(PickGmdFileToLink);
            AutoSelectFromFolderCommand = ReactiveCommand.CreateFromTask(AutoSelectFromFolder);
            AutoSelectFromOpenedFilesCommand = ReactiveCommand.Create(AutoSelectFromOpenedFiles);

            mainWindowViewModel.openFilesList.Connect()
                .Filter(x => x is GmdViewModel)
                .Transform(x => (GmdViewModel)x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out openGmdFilesBinding)
                .DisposeMany()
                .Subscribe();

            csvEntries.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out csvEntriesBinding)
                .DisposeMany()
                .Subscribe();

            importEntries.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out importEntriesBinding)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedGmdFile, x => x.HideUnchangedEntries)
                .Where(x => x.Item1 != null)
                .Select(_ => Unit.Default)
                .InvokeCommand(PopulateImportEntriesCommand);

            ReloadCsvCommand.Execute().Subscribe();
        }

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;

        public IList<FileDialogFilter> SaveFileDialogFilters { get; } = new[] { FileDialogFilter.CSV };

        public ReactiveCommand<Unit, Unit> ReloadCsvCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyAndSaveCommand { get; }
        public ReactiveCommand<Unit, Unit> PopulateImportEntriesCommand { get; }
        public ReactiveCommand<Unit, Unit> PickGmdFileToLinkCommand { get; }
        public ReactiveCommand<Unit, Unit> AutoSelectFromFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> AutoSelectFromOpenedFilesCommand { get; }

        [Reactive] public bool HideUnchangedEntries { get; set; }
        [Reactive] public GmdViewModel? SelectedGmdFile { get; set; }

        public ReadOnlyObservableCollection<StringKeyValuePair> CsvEntries => csvEntriesBinding;
        public ReadOnlyObservableCollection<GmdEntryDiffViewModel> ImportEntries => importEntriesBinding;
        public ReadOnlyObservableCollection<GmdViewModel> OpenGmdFiles => openGmdFilesBinding;

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ReadOnlyObservableCollection<StringKeyValuePair> csvEntriesBinding;
        private readonly ReadOnlyObservableCollection<GmdEntryDiffViewModel> importEntriesBinding;
        private readonly ReadOnlyObservableCollection<GmdViewModel> openGmdFilesBinding;
        private readonly SourceCache<StringKeyValuePair, string> csvEntries = new SourceCache<StringKeyValuePair, string>(x => x.Key);
        private readonly SourceCache<GmdEntryDiffViewModel, string> importEntries = new SourceCache<GmdEntryDiffViewModel, string>(x => x.Entry.Key!);
        private static readonly ILogger log = Log.ForContext<GmdCsvViewModel>();

        public bool Close()
        {
            return true;
        }

        public Task Save(string path)
        {
            throw new NotImplementedException();
        }

        private void LoadCsv(string path)
        {
            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using TextReader tr = new StreamReader(fs, ExEncoding.UTF8);
            using CsvReader csv = new CsvReader(tr, CultureInfo.InvariantCulture);

            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ";";
            csv.Configuration.AllowComments = true; // Uses # to identify comments

            var values = csv.GetRecords<StringKeyValuePair>();
            csvEntries.Edit(x => x.Load(values));
            PopulateImportEntriesCommand.Execute().Subscribe();
        }

        private async Task ApplyAndSave()
        {
            if (SelectedGmdFile == null) return;

            int updatedEntriesCount = 0;

            foreach (var entry in ImportEntries)
            {
                if (entry.ApplyChanges())
                    updatedEntriesCount++;
            }

            if (updatedEntriesCount > 0)
            {
                await mainWindowViewModel.SaveFilesCommand.Execute(new[] { SelectedGmdFile });
                mainWindowViewModel.ShowFlashAlert("Updated entries", $"Changed {updatedEntriesCount} entries in {SelectedGmdFile.Info.FullName}");
            }
            else
            {
                mainWindowViewModel.ShowFlashAlert("No entries changed", $"No entries have been modified in {SelectedGmdFile.Info.FullName}");
            }

            log.Information($"Applied {updatedEntriesCount} changes from '{Info.FullName}' to '{SelectedGmdFile.Info.FullName}'");
        }

        // This function can also be replaced by a DynamicData observable list, but I don't know how.
        private void PopulateImportEntries()
        {
            // TODO: This function might be pretty slow. Maybe run on background thread?
            if (SelectedGmdFile == null)
            {
                log.Warning("Can't execute 'PopulateImportEntries' when 'SelectedGmdFile' is null.");
                return;
            }

            var sw = Stopwatch.StartNew();

            importEntries.Edit(lst =>
            {
                lst.Clear();

                // Not sure what this code does, but it seems to work...
                var entries = SelectedGmdFile.Connect().AsObservableList().Items.ToList();

                foreach (var entry in entries.Where(x => x.Key != null))
                {
                    // TODO: Allow user to deselect CsvEntry (to not import that entry)
                    var newValue = CsvEntries.FirstOrDefault(x => x.Key == entry.Key);

                    if (newValue != null)
                    {
                        var diff = new GmdEntryDiffViewModel(entry, newValue.Value);

                        if (!HideUnchangedEntries || diff.HasChanges)
                            lst.AddOrUpdate(new GmdEntryDiffViewModel(entry, newValue.Value));
                    }
                }
            });

            sw.Stop();

            log.Verbose("PopulateImportEntries took {@ElapsedMilliseconds} ms.", sw.ElapsedMilliseconds);
        }

        private async Task PickGmdFileToLink()
        {
            var openedFiles = await mainWindowViewModel.OpenFileCommand.Execute(new[] { "gmd" });

            if (openedFiles.Count > 1)
            {
                mainWindowViewModel.ShowFlashAlert("Can't link GMD", "You opened multiple GMD files and I don't know which one you want to link.\nPlease manually select the desired GMD file in the drop down.");
            }
            else if (openedFiles.Count == 1 && openedFiles[0] is GmdViewModel vm)
            {
                SelectedGmdFile = vm;
            }
        }

        private async Task AutoSelectFromFolder()
        {
            var folder = await mainWindowViewModel.Framework.OpenFolderDialog();
            if (folder != null)
            {
                string expectedName = $"{Path.GetFileNameWithoutExtension(Info.Name)}.gmd";
                var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                var matchingFile = files.FirstOrDefault(x => x.EndsWith(expectedName));

                if (matchingFile != null)
                {
                    var openedFiles = mainWindowViewModel.OpenFiles(new[] { matchingFile });
                    if (openedFiles.Count == 1 && openedFiles[0] is GmdViewModel gmdViewModel)
                        SelectedGmdFile = gmdViewModel;
                }
                else
                {
                    mainWindowViewModel.ShowFlashAlert("Couldn't Auto-Select from Folder", $"No file in {folder} (or its subdirectories) matches the expected filename: {expectedName}");
                }
            }
        }

        private void AutoSelectFromOpenedFiles()
        {
            string expectedName = $"{Path.GetFileNameWithoutExtension(Info.Name)}.gmd";
            var matchingGmd = OpenGmdFiles.FirstOrDefault(x => x.Info.Name == expectedName);

            if (matchingGmd != null)
            {
                SelectedGmdFile = matchingGmd;
            }
            else
            {
                mainWindowViewModel.ShowFlashAlert("Couldn't Auto-Select from Opened Files", $"No opened file matches the expected filename: {expectedName}");
            }
        }
    }
}
