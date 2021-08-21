using Cirilla.Core.Models;
using Cirilla.MVVM.Common;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public enum BatchConversion
    {
        GmdToCsv,
        CsvToGmd,
    }

    public class BatchFileEntryViewModel : ViewModelBase
    {
        public BatchFileEntryViewModel(BatchConversion conversion, FileInfo info)
        {
            this.conversion = conversion;

            Name = info.Name;
            FullPath = info.FullName;

            this.WhenAnyValue(x => x.OutputDirectory)
                .Select(GetOutputFullPath)
                .ToPropertyEx(this, x => x.Target);
        }

        public string Name { get; }
        public string FullPath { get; }

        private readonly BatchConversion conversion;

        [Reactive] public string OutputDirectory { get; set; } = "";
        [ObservableAsProperty] public string Target { get; } = "";

        private string GetOutputFullPath(string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory)) return "";
            var ext = conversion == BatchConversion.CsvToGmd ? ".gmd" : ".csv";
            return Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(Name) + ext);
        }
    }

    public class CsvToGmdBatchFileEntryViewModel : BatchFileEntryViewModel
    {
        public CsvToGmdBatchFileEntryViewModel(FileInfo info) : base(BatchConversion.CsvToGmd, info)
        {
            this.WhenAnyValue(x => x.GmdSourceFolder)
                .Select(GetGmdSourcePath)
                .ToPropertyEx(this, x => x.GmdSourcePath);
        }

        [Reactive] public string GmdSourceFolder { get; set; } = "";
        [ObservableAsProperty] public string? GmdSourcePath { get; set; }

        private static readonly ILogger log = Log.ForContext<CsvToGmdBatchFileEntryViewModel>();

        private string? GetGmdSourcePath(string gmdSourceDirectory)
        {
            if (string.IsNullOrWhiteSpace(gmdSourceDirectory)) return null;

            var gmdName = Path.GetFileNameWithoutExtension(Name) + ".gmd";
            var gmdFiles = Directory.GetFiles(gmdSourceDirectory, gmdName, SearchOption.AllDirectories).ToArray();

            if (gmdFiles.Length == 0)
            {
                log.Warning($"Couldn't find GMD file '{gmdName}' inside the GMD source directory.");
                return null;
            }
            else if (gmdFiles.Length > 1)
            {
                int best = 0;
                string? bestPath = null;
                string wantedPath = FullPath.Replace(".csv", ".gmd");

                foreach (var file in gmdFiles)
                {
                    var score = CalculatePathLikeness(wantedPath, file);
                    if (score > best)
                    {
                        bestPath = file;
                        best = score;
                    }
                }

                return bestPath;
            }
            else
            {
                return gmdFiles[0];
            }
        }

        private static int CalculatePathLikeness(string a, string b)
        {
            int score = 0;
            int len = Math.Min(a.Length, b.Length) + 1;

            for (int i = 1; i < len; i++)
            {
                if (a[a.Length - i] == b[b.Length - 1]) score++;
                else break;
            }
            return score;
        }
    }

    public class GmdCsvBatchToolViewModel : ViewModelBase, IExplorerItem
    {
        public GmdCsvBatchToolViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            ShowAddFileDialogCommand = ReactiveCommand.CreateFromTask(ShowAddFileDialog);
            ShowAddFolderDialogCommand = ReactiveCommand.CreateFromTask<bool>(ShowAddFolderDialog);
            ClearAllCommand = ReactiveCommand.Create(ClearAllItems);
            ShowBrowseOutputFolderDialogCommand = ReactiveCommand.CreateFromTask(ShowBrowseOutputFolderDialog);
            ShowBrowseGmdSourceFolderDialogCommand = ReactiveCommand.CreateFromTask(ShowBrowseGmdSourceFolderDialog);
            ExecuteBatchToolCommand = ReactiveCommand.CreateFromTask(ExecuteBatchTool);

            gmdFilesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out gmdFilesBinding)
                .DisposeMany()
                .Subscribe();

            csvFilesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out csvFilesBinding)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(x => x.IsGmdToCsvSelected)
                .Select(x => x ? "Export All To CSV" : "Apply All To GMD")
                .ToPropertyEx(this, x => x.ExportButtonText);

            // Shitty workaround for WPF not allowing us to invert a bool, nor allowing us to use multiple ValueConverters
            this.WhenAnyValue(x => x.IsGmdToCsvSelected)
                .Select(x => !x)
                .ToPropertyEx(this, x => x.IsCsvToGmdSelected);

            this.WhenAnyValue(x => x.GmdToCsvOutputFolder)
                .Subscribe(x =>
                {
                    if (Directory.Exists(x))
                    {
                        foreach (var entry in gmdFilesList.Items)
                            entry.OutputDirectory = x;
                    }
                });

            this.WhenAnyValue(x => x.CsvToGmdOutputFolder)
                .Subscribe(x =>
                {
                    if (Directory.Exists(x))
                    {
                        foreach (var entry in csvFilesList.Items)
                            entry.OutputDirectory = x;
                    }
                });

            this.WhenAnyValue(x => x.GmdSourceFolder)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(x =>
                {
                    if (Directory.Exists(x))
                    {
                        var alert = mainWindowViewModel.ShowFlashAlert("Matching GMD files to CSV files", "This can take up to a minute depending on how many CSV files you have.", buttons: FlashMessageButtons.None);
                        Parallel.ForEach(csvFilesList.Items, (entry) => entry.GmdSourceFolder = x);
                        alert.Close();
                    }
                });

            this.WhenAnyValue(x => x.IsGmdToCsvSelected, x => x.GmdFilesBinding.Count, x => x.CsvFilesBinding.Count)
                .Select(x => x.Item1 ? x.Item2 : x.Item3)
                .Select(x => x == 0 ? "No Items" : $"{x} Items")
                .ToPropertyEx(this, x => x.SelectedItemsCountText);
        }

        public ReactiveCommand<Unit, Unit> ShowAddFileDialogCommand { get; }
        public ReactiveCommand<bool, Unit> ShowAddFolderDialogCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowBrowseOutputFolderDialogCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowBrowseGmdSourceFolderDialogCommand { get; }
        public ReactiveCommand<Unit, Unit> ExecuteBatchToolCommand { get; }

        public ReadOnlyObservableCollection<BatchFileEntryViewModel> GmdFilesBinding => gmdFilesBinding;
        public ReadOnlyObservableCollection<CsvToGmdBatchFileEntryViewModel> CsvFilesBinding => csvFilesBinding;

        [Reactive] public bool IsGmdToCsvSelected { get; set; } = true;
        [Reactive] public string GmdToCsvOutputFolder { get; set; } = "";
        [Reactive] public string CsvToGmdOutputFolder { get; set; } = "";
        [Reactive] public string GmdSourceFolder { get; set; } = "";

        [ObservableAsProperty] public bool IsCsvToGmdSelected { get; } = false;
        [ObservableAsProperty] public string ExportButtonText { get; } = null!;
        [ObservableAsProperty] public string SelectedItemsCountText { get; } = null!;

        public string Title { get; } = "Batch GMD/CSV Operations";
        public string StatusText { get; } = "";
        public bool CanClose { get; } = true;

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ReadOnlyObservableCollection<BatchFileEntryViewModel> gmdFilesBinding;
        private readonly ReadOnlyObservableCollection<CsvToGmdBatchFileEntryViewModel> csvFilesBinding;
        private readonly SourceList<BatchFileEntryViewModel> gmdFilesList = new();
        private readonly SourceList<CsvToGmdBatchFileEntryViewModel> csvFilesList = new();
        private static readonly ILogger log = Log.ForContext<GmdCsvBatchToolViewModel>();

        public bool Close()
        {
            return true;
        }

        private async Task ShowAddFileDialog()
        {
            if (IsGmdToCsvSelected)
            {
                var filters = new List<FileDialogFilter> { FileDialogFilter.GMD };
                var files = await mainWindowViewModel.Framework.OpenFileDialog(true, filters);

                if (files.Length > 0)
                {
                    await AddGmdToCsvFiles(files);

                    if (GmdSourceFolder == "")
                        GmdSourceFolder = Path.GetDirectoryName(files[0]);
                }
            }
            else
            {
                var filters = new List<FileDialogFilter> { FileDialogFilter.CSV };
                var files = await mainWindowViewModel.Framework.OpenFileDialog(true, filters);
                await AddCsvToGmdFiles(files);
            }
        }

        private async Task ShowAddFolderDialog(bool recursive)
        {
            var opts = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var folder = await mainWindowViewModel.Framework.OpenFolderDialog();

            if (folder != null)
            {
                if (IsGmdToCsvSelected)
                {
                    var alert = mainWindowViewModel.ShowFlashAlert("Searching for files...", $"Searching for GMD files inside '{folder}'", buttons: FlashMessageButtons.None);
                    var files = await Task.Run(() => Directory.GetFiles(folder, "*.gmd", opts));
                    alert.Close();

                    await AddGmdToCsvFiles(files);

                    if (GmdSourceFolder == "")
                        GmdSourceFolder = folder;
                }
                else
                {
                    var alert = mainWindowViewModel.ShowFlashAlert("Searching for files...", $"Searching for CSV files inside '{folder}'", buttons: FlashMessageButtons.None);
                    var files = await Task.Run(() => Directory.GetFiles(folder, "*.csv", opts));
                    alert.Close();

                    await AddCsvToGmdFiles(files);
                }
            }
        }

        private void ClearAllItems()
        {
            if (IsGmdToCsvSelected)
            {
                gmdFilesList.Clear();
            }
            else
            {
                csvFilesList.Clear();
            }
        }

        private async Task ShowBrowseOutputFolderDialog()
        {
            var folder = await mainWindowViewModel.Framework.OpenFolderDialog();
            if (folder != null)
            {
                if (IsGmdToCsvSelected)
                {
                    GmdToCsvOutputFolder = folder;
                }
                else
                {
                    CsvToGmdOutputFolder = folder;
                }
            }
        }

        private async Task ShowBrowseGmdSourceFolderDialog()
        {
            var folder = await mainWindowViewModel.Framework.OpenFolderDialog();
            if (folder != null)
            {
                GmdSourceFolder = folder;
            }
        }

        private async Task ExecuteBatchTool()
        {
            var outputFolder = IsGmdToCsvSelected ? GmdToCsvOutputFolder : CsvToGmdOutputFolder;
            var entryListCount = IsGmdToCsvSelected ? gmdFilesList.Count : csvFilesList.Count;

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                mainWindowViewModel.ShowFlashAlert("Output folder unset", "Please pick a folder where you want the exported files to be saved to.");
                return;
            }

            if (entryListCount == 0)
            {
                mainWindowViewModel.ShowFlashAlert("No files selected", "Use the 'Add File' or 'Add Folder' buttons to select which files you want to convert.");
                return;
            }

            // Check for overlapping output files
            var targets = IsGmdToCsvSelected ? gmdFilesList.Items.Select(x => x.Target).ToList() : csvFilesList.Items.Select(x => x.Target).ToList();
            int dupeCount = targets.Count - targets.Distinct().Count();
            if (dupeCount != 0)
            {
                var duplicates = targets.GroupBy(x => x).Where(x => x.Count() > 1).Take(10).Select(x => $"- {x.Key}");
                var duplicatesString = string.Join("\n", duplicates);

                if (dupeCount > 10) duplicatesString += $"\n- and {dupeCount - 10} more";

                mainWindowViewModel.ShowFlashAlert("Duplicate output files", $"You have {dupeCount} files which have the same output filename. Please fix this.\n\nDuplicates:\n{duplicatesString}");
                return;
            }

            await Task.Run(() =>
            {
                if (IsGmdToCsvSelected)
                {
                    int counter = 1;
                    var alert = mainWindowViewModel.ShowFlashAlert("Exporting GMD to CSV...", buttons: FlashMessageButtons.None);

                    // Parallel this?
                    foreach (var entry in gmdFilesList.Items)
                    {
                        try
                        {
                            alert.Title = $"Exporting GMD to CSV ({counter++}/{gmdFilesList.Count})";
                            var gmd = new GMD(entry.FullPath);
                            gmd.ExportToCsv(entry.Target);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, $"Error when exporting {entry.FullPath} to CSV.");
                        }
                    }

                    alert.Close();
                    mainWindowViewModel.ShowFlashAlert($"Exported {gmdFilesList.Count} GMD files to CSV", $"You can find all exported files inside the '{GmdToCsvOutputFolder}' directory.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(GmdSourceFolder))
                    {
                        mainWindowViewModel.ShowFlashAlert("GMD source folder unset", "Please pick the GMD source folder. The files in this folder will not be changed, only be used as a base to where we apply our CSV.");
                        return;
                    }

                    int i = 1;
                    int changedEntries = 0;
                    int processedFiles = 0;
                    var alert = mainWindowViewModel.ShowFlashAlert("Applying CSV to GMD...", buttons: FlashMessageButtons.None);

                    // Parallel this?
                    foreach (var entry in csvFilesList.Items)
                    {
                        try
                        {
                            alert.Title = $"Applying CSV to GMD ({i++}/{csvFilesList.Count})";

                            if (entry.GmdSourcePath != null)
                            {
                                var gmd = new GMD(entry.GmdSourcePath);
                                var updateCount = gmd.ImportFromCsv(entry.FullPath);
                                changedEntries += updateCount;
                                processedFiles++;

                                if (updateCount > 0)
                                {
                                    gmd.Save(entry.Target);
                                    changedEntries += updateCount;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, $"Error when importing '{entry.FullPath}' into '{entry.GmdSourcePath}'.");
                        }
                    }

                    alert.Close();
                    mainWindowViewModel.ShowFlashAlert($"Processed {processedFiles}/{csvFilesList.Count} CSV files", $"Updated {changedEntries} values inside {processedFiles} GMD files.\nYou can find all updated GMD files inside the '{CsvToGmdOutputFolder}' directory.");
                }
            });
        }

        private async Task AddGmdToCsvFiles(string[] files)
        {
            var toAdd = files
                // Don't add dupes
                .Except(gmdFilesList.Items.Select(x => x.FullPath))
                .Select(x => new BatchFileEntryViewModel(BatchConversion.GmdToCsv, new FileInfo(x)))
                .ToList();

            gmdFilesList.AddRange(toAdd);

            await Task.Run(() =>
            {
                var alert = mainWindowViewModel.ShowFlashAlert("Loading GMD files...", buttons: FlashMessageButtons.None);
                foreach (var entry in toAdd)
                {
                    entry.OutputDirectory = GmdToCsvOutputFolder;
                }
                alert.Close();
            });
        }

        private async Task AddCsvToGmdFiles(string[] files)
        {
            var toAdd = files
                // Don't add dupes
                .Except(csvFilesList.Items.Select(x => x.FullPath))
                .Select(x => new CsvToGmdBatchFileEntryViewModel(new FileInfo(x)))
                .ToList();

            csvFilesList.AddRange(toAdd);

            await Task.Run(() =>
            {
                var alert = mainWindowViewModel.ShowFlashAlert("Matching GMD files to CSV files...", "This can take up to a minute depending on how many CSV files you have.", buttons: FlashMessageButtons.None);
                foreach (var entry in toAdd)
                {
                    entry.GmdSourceFolder = GmdSourceFolder;
                    entry.OutputDirectory = CsvToGmdOutputFolder;
                }
                alert.Close();
            });
        }
    }
}
