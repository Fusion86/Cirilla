using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Cirilla.MVVM.Services;
using DynamicData;
using DynamicData.Binding;
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
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IFlashMessageContainer
    {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public MainWindowViewModel() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public MainWindowViewModel(IFrameworkService framework, LogCollector? logCollector = null)
        {
            Framework = framework;
            this.logCollector = logCollector;

            RxApp.DefaultExceptionHandler = new ExceptionHandler(this);

            if (logCollector != null)
                logViewerViewModel = new LogViewerViewModel(logCollector);

            // True when one or more items have been selected in the openFilesList.
            var hasSelectedFile = this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null && typeof(IOpenFileViewModel).IsAssignableFrom(x.GetType()));

            // True when the OpenFiles list has at least 1 item.
            var hasFiles = sidebarItemsList.CountChanged.Select(x => x > 0).ObserveOn(RxApp.MainThreadScheduler);

            OpenFileCommand = ReactiveCommand.CreateFromTask<string[]?, IList<IOpenFileViewModel>>(ShowOpenFileDialog);
            OpenFolderCommand = ReactiveCommand.CreateFromTask(ShowOpenFolderDialog);
            SaveSelectedFileCommand = ReactiveCommand.CreateFromTask(SaveSelectedFile, hasSelectedFile);
            SaveSelectedFileAsCommand = ReactiveCommand.CreateFromTask(SaveSelectedFileAs, hasSelectedFile);
            SaveAllFilesCommand = ReactiveCommand.CreateFromTask(SaveAllFiles, hasFiles);
            CloseSelectedFileCommand = ReactiveCommand.Create(CloseSelectedFile, hasSelectedFile);
            CloseAllFilesCommand = ReactiveCommand.Create(CloseAllFiles, hasFiles);
            ShowLogViewerCommand = ReactiveCommand.Create(ShowLogViewer);
            OpenBatchImportCommand = ReactiveCommand.Create(OpenBatchImport);

            CloseFileCommand = ReactiveCommand.Create<IOpenFileViewModel>(CloseFile);
            // The below commands have a hasSelectedFile because they always get openFileListBox.SelectedItems passed as CommandParameter
            SaveFilesCommand = ReactiveCommand.CreateFromTask<IList<IOpenFileViewModel>>(SaveFiles, hasSelectedFile);
            SaveFilesAsCommand = ReactiveCommand.CreateFromTask<IList<IOpenFileViewModel>>(SaveFilesAs, hasSelectedFile);
            CloseFilesCommand = ReactiveCommand.Create<IList<IOpenFileViewModel>>(CloseFiles, hasSelectedFile);

            sidebarItemsList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out sidebarItemsBinding)
                .DisposeMany()
                .Subscribe();

            flashMessages.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out flashMessagesBinding)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => x == null ? "Cirilla Toolkit" : $"{x.Title} - Cirilla Toolkit")
                .ToPropertyEx(this, x => x.Title);

            var shouldShowFlashAlert = flashMessages.CountChanged
                .Select(x => x > 0);

            shouldShowFlashAlert
                .Select(x => x ? (double)framework.FindResource("ThemeDisabledOpacity")! : 1.0)
                .ToPropertyEx(this, x => x.ContentOpacity);

            this.WhenAnyValue(x => x.SelectedItem)
                .Skip(1) // Skip initial value
                .Subscribe(x => ContentViewModel = x);

            if (this.logCollector != null)
            {
                this.logCollector.Events
                    .ToObservableChangeSet()
                    .OnItemAdded(x => StatusText = x.Message)
                    .Subscribe();
            }
        }

        public ReactiveCommand<string[]?, IList<IOpenFileViewModel>> OpenFileCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowLogViewerCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSelectedFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSelectedFileAsCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllFilesCommand { get; } // We can't use SaveFilesCommand because we want to use the CanExecute property
        public ReactiveCommand<Unit, Unit> CloseSelectedFileCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseAllFilesCommand { get; } // We can't use CloseFilesCommand because we want to use the CanExecute property
        public ReactiveCommand<Unit, Unit> OpenBatchImportCommand { get; }

        public ReactiveCommand<IOpenFileViewModel, Unit> CloseFileCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> SaveFilesCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> SaveFilesAsCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> CloseFilesCommand { get; }

        public ReadOnlyObservableCollection<ISidebarItemViewModel> SidebarItemsBinding => sidebarItemsBinding;
        public ReadOnlyObservableCollection<FlashMessageViewModel> FlashMessages => flashMessagesBinding;

        [Reactive] public string StatusText { get; set; } = "";
        [Reactive] public ISidebarItemViewModel? SelectedItem { get; set; }
        [Reactive] public ITitledViewModel? ContentViewModel { get; set; }

        [ObservableAsProperty] public string? Title { get; }
        [ObservableAsProperty] public double ContentOpacity { get; }

        public IFrameworkService Framework { get; }

        private static readonly ILogger log = Log.ForContext<MainWindowViewModel>();
        private readonly LogCollector? logCollector;
        private readonly LogViewerViewModel? logViewerViewModel;
        private readonly ReadOnlyObservableCollection<ISidebarItemViewModel> sidebarItemsBinding;
        private readonly ReadOnlyObservableCollection<FlashMessageViewModel> flashMessagesBinding;
        internal readonly SourceList<ISidebarItemViewModel> sidebarItemsList = new SourceList<ISidebarItemViewModel>();
        private readonly SourceList<FlashMessageViewModel> flashMessages = new SourceList<FlashMessageViewModel>();

        private async Task<IList<IOpenFileViewModel>> ShowOpenFileDialog(string[]? allowedExtensions = null)
        {
            var filters = new List<FileDialogFilter> { FileDialogFilter.AllFiles, FileDialogFilter.GMD, FileDialogFilter.CSV };

            if (allowedExtensions != null)
            {
                // Only keep filters that contain atleast one of the extensions in 'allowedExtensions'.
                filters.RemoveAll(x =>
                {
                    var obj = x.Extensions.FirstOrDefault(x => allowedExtensions.Contains(x));
                    return obj == null; // Remove item if 'Extensions' does not contain one of the 'allowedExtensions'.
                });
            }

            var files = await Framework.OpenFileDialog(true, filters);

            return await Task.Run(() => OpenFiles(files));
        }

        private async Task ShowOpenFolderDialog()
        {
            var folder = await Framework.OpenFolderDialog();
            if (folder != null)
            {
                var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                log.Information($"Opening folder '{folder}' which has {files.Length} files...");

                await Task.Run(() => OpenFiles(files));
            }
        }

        private async Task SaveSelectedFile()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                await SaveFile(openFile, showMessageOnSuccess: true);
            else
                log.Warning("No saveable item selected.");
        }

        private async Task SaveSelectedFileAs()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                await SaveFileAs(openFile, true);
            else
                log.Warning("No saveable item selected.");
        }

        private async Task SaveAllFiles()
        {
            await SaveFiles(SidebarItemsBinding.OfType<IOpenFileViewModel>().ToList());
        }

        private void CloseSelectedFile()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                CloseFile(openFile);
            else
                log.Warning("No closeable item selected.");
        }

        private void CloseAllFiles()
        {
            foreach (var file in SidebarItemsBinding)
                CloseFile(file);
        }

        private void ShowLogViewer()
        {
            if (logCollector != null)
            {
                if (ContentViewModel != logViewerViewModel)
                    ContentViewModel = logViewerViewModel;
                else
                    ContentViewModel = SelectedItem;
            }
            else
                log.Warning("Can't create LogViewerViewMOdel because no LogCollector was passed to the MainWindowViewModel.");
        }

        private async Task SaveFiles(IList<IOpenFileViewModel> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                log.Warning("No items selected.");
            }
            else
            {
                var showSuccessForEachFile = lst.Count == 1;
                int fileSaveCount = 0;
                foreach (var file in lst)
                {
                    if (await SaveFile(file, showMessageOnSuccess: showSuccessForEachFile))
                        fileSaveCount++;
                }

                if (!showSuccessForEachFile)
                {
                    ShowFlashAlert($"Saved {fileSaveCount} files", $"Successfully saved {fileSaveCount} files.");
                }
            }
        }

        private async Task SaveFilesAs(IList<IOpenFileViewModel> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                log.Warning("No items selected.");
            }
            else
            {
                var showSuccessForEachFile = lst.Count == 1;
                int fileSaveCount = 0;
                foreach (var file in lst)
                {
                    if (await SaveFileAs(file, showMessageOnSuccess: showSuccessForEachFile))
                        fileSaveCount++;
                }

                if (!showSuccessForEachFile)
                {
                    ShowFlashAlert($"Saved {fileSaveCount} files", $"Successfully saved {fileSaveCount} files.");
                }
            }
        }

        private void CloseFiles(IList<IOpenFileViewModel> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                log.Warning("No items selected.");
            }
            else
            {
                foreach (var file in lst)
                    CloseFile(file);
            }
        }

        public IList<IOpenFileViewModel> OpenFiles(string[] files)
        {
            var openedFiles = new List<IOpenFileViewModel>();
            var alert = ShowFlashAlert(buttons: FlashMessageButtons.None);

            for (int i = 0; i < files.Length; i++)
            {
                var fileInfo = new FileInfo(files[i]);
                alert.Title = $"Loading file {i + 1}/{files.Length}";
                alert.Message = $"Loading {fileInfo.FullName} ...";

                var vm = TryCreateViewModelForFile(fileInfo);

                if (vm != null)
                {
                    sidebarItemsList.Add(vm);
                    openedFiles.Add(vm);
                }
            }

            alert.Close();

            return openedFiles;
        }

        /// <summary>
        /// Shows SaveFileDialog and saves file when user clicks ok.
        /// </summary>
        /// <param name="openFile"></param>
        /// <returns></returns>
        public async Task<bool> SaveFileAs(IOpenFileViewModel openFile, bool showMessageOnSuccess = false)
        {
            var name = await Framework.SaveFileDialog(openFile.Info.Name, openFile.Info.Extension, openFile.SaveFileDialogFilters);

            if (name != null)
                return await SaveFile(openFile, name, showMessageOnSuccess);
            return false;
        }

        /// <summary>
        /// Checks if passed file CanClose, calls the Close() method on the file, and removes it from the openFilesList.
        /// </summary>
        /// <param name="vm"></param>
        public void CloseFile(ISidebarItemViewModel vm)
        {
            if (vm.CanClose && vm.Close())
            {
                SelectedItem = null;
                sidebarItemsList.Remove(vm);
            }
            else
            {
                log.Warning("Couldn't close file.");
            }
        }

        /// <summary>
        /// Saves the file to the given savePath. If savePath is left empty the file will be saved in-place (overwriting the old file).
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath">Where to save the file. If left empty the file will be saved in-place (overwriting the old file).</param>
        public async Task<bool> SaveFile(IOpenFileViewModel file, string? savePath = null, bool showMessageOnSuccess = false)
        {
            bool isSuccess = false;

            if (savePath == null)
            {
                savePath = file.Info.FullName;
            }
            else if (string.IsNullOrWhiteSpace(savePath))
            {
                log.Warning("Can't save file because savePath is empty or whitespace (but not null).");
            }

            var alert = ShowFlashAlert($"Saving {savePath} ...", buttons: FlashMessageButtons.None);

            try
            {
                await file.Save(savePath);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Could not save file.");
                ShowFlashAlert("Could not save file!", ex.Message, FlashMessageButtons.Ok);
            }

            alert.Close();
            return isSuccess;
        }

        private IOpenFileViewModel? TryCreateViewModelForFile(FileInfo fileInfo)
        {
            try
            {
                //var gmd = new GmdViewModel(fileInfo);
                //return gmd;

                return fileInfo.Extension.ToLowerInvariant() switch
                {
                    ".gmd" => new GmdViewModel(fileInfo, this),
                    ".csv" => new CsvViewModel(fileInfo, this),
                    _ => throw new NotSupportedException($"{fileInfo.FullName} is not a suported file type.")
                };
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error when opening file.");
                ShowFlashAlert("Error when opening file", ex.Message, FlashMessageButtons.Ok);
                return null;
            }
        }

        private void OpenBatchImport()
        {
            sidebarItemsList.Add(new BatchImportViewModel());
        }

        public FlashMessageViewModel ShowFlashAlert(string title = "", string message = "", FlashMessageButtons buttons = FlashMessageButtons.Ok)
        {
            var alert = new FlashMessageViewModel(title, message, buttons);
            alert.OnClose += (sender, e) => flashMessages.Remove(alert);
            flashMessages.Insert(0, alert);
            return alert;
        }
    }
}
