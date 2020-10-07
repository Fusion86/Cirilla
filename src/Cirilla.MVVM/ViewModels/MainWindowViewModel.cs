using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Cirilla.MVVM.Services;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public MainWindowViewModel() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public MainWindowViewModel(IFrameworkService framework, LogCollector? logCollector = null)
        {
            this.framework = framework;
            this.logCollector = logCollector;

            // True when one or more items have been selected in the openFilesList.
            var hasSelectedFile = this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null && typeof(IOpenFileViewModel).IsAssignableFrom(x.GetType()));

            // True when the OpenFiles list has at least 1 item.
            var hasFiles = openFilesList.CountChanged.Select(x => x > 0).ObserveOn(RxApp.MainThreadScheduler);

            OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFileCommandHandler);
            OpenFolderCommand = ReactiveCommand.CreateFromTask(OpenFolderHandler);
            SaveSelectedFileCommand = ReactiveCommand.CreateFromTask(SaveSelectedFileHandler, hasSelectedFile);
            SaveSelectedFileAsCommand = ReactiveCommand.CreateFromTask(SaveSelectedFileAsHandler, hasSelectedFile);
            SaveAllFilesCommand = ReactiveCommand.CreateFromTask(SaveAllFilesHandler, hasFiles);
            CloseSelectedFileCommand = ReactiveCommand.Create(CloseSelectedFileHandler, hasSelectedFile);
            CloseAllFilesCommand = ReactiveCommand.Create(CloseAllFilesHandler, hasFiles);
            ShowLogViewerCommand = ReactiveCommand.Create(ShowLogViewerHandler);

            CloseFileCommand = ReactiveCommand.Create<IOpenFileViewModel>(CloseFileHandler);
            // The below commands have a hasSelectedFile because they always get openFileListBox.SelectedItems passed as CommandParameter
            SaveFilesCommand = ReactiveCommand.CreateFromTask<IList<IOpenFileViewModel>>(SaveFilesHandler, hasSelectedFile);
            SaveFilesAsCommand = ReactiveCommand.CreateFromTask<IList<IOpenFileViewModel>>(SaveFilesAsHandler, hasSelectedFile);
            CloseFilesCommand = ReactiveCommand.Create<IList<IOpenFileViewModel>>(CloseFilesHandler, hasSelectedFile);

            openFilesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out openFilesBinding)
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

            if (this.logCollector != null)
            {
                // HACK: This code is absolutely tragic, but I haven't found a better way to do this.
                ((INotifyCollectionChanged)this.logCollector.Events).CollectionChanged += (sender, e) =>
                {
                    if (e.NewItems != null
                        && e.NewItems.Count != 0
                        && e.NewItems[e.NewItems.Count - 1] is LogEventViewModel logEvent)
                        StatusText = logEvent.Message;
                };
            }
        }

        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowLogViewerCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSelectedFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSelectedFileAsCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveAllFilesCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseSelectedFileCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseAllFilesCommand { get; }

        public ReactiveCommand<IOpenFileViewModel, Unit> CloseFileCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> SaveFilesCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> SaveFilesAsCommand { get; }
        public ReactiveCommand<IList<IOpenFileViewModel>, Unit> CloseFilesCommand { get; }

        public ReadOnlyObservableCollection<IOpenFileViewModel> OpenFilesBinding => openFilesBinding;
        public ReadOnlyObservableCollection<FlashMessageViewModel> FlashMessages => flashMessagesBinding;

        [Reactive] public string StatusText { get; set; } = "";
        [Reactive] public ITitledViewModel? SelectedItem { get; set; }

        [ObservableAsProperty] public string? Title { get; }
        [ObservableAsProperty] public double ContentOpacity { get; }

        private static readonly ILogger logger = Log.ForContext<MainWindowViewModel>();
        private readonly IFrameworkService framework;
        private readonly LogCollector? logCollector;
        private readonly ReadOnlyObservableCollection<IOpenFileViewModel> openFilesBinding;
        private readonly ReadOnlyObservableCollection<FlashMessageViewModel> flashMessagesBinding;
        private readonly SourceList<IOpenFileViewModel> openFilesList = new SourceList<IOpenFileViewModel>();
        private readonly SourceList<FlashMessageViewModel> flashMessages = new SourceList<FlashMessageViewModel>();

        private static readonly ILogger log = Log.ForContext<MainWindowViewModel>();

        private async Task OpenFileCommandHandler()
        {
            var filters = new List<FileDialogFilter> { new FileDialogFilter("GMD Text File", new List<string> { "gmd" }) };
            var files = await framework.OpenFileDialog(true, filters);

            await Task.Run(() => OpenFiles(files));
        }

        private async Task OpenFolderHandler()
        {
            var folder = await framework.OpenFolderDialog();
            if (folder != null)
            {
                var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                log.Information($"Opening folder '{folder}' which has {files.Length} files...");

                await Task.Run(() => OpenFiles(files));
            }
        }

        private async Task SaveSelectedFileHandler()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                await SaveFile(openFile);
            else
                log.Warning("No saveable item selected.");
        }

        private async Task SaveSelectedFileAsHandler()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                await SaveFileAs(openFile);
            else
                log.Warning("No saveable item selected.");
        }

        private async Task SaveAllFilesHandler()
        {
            // TODO: Add cancel button
            var alert = ShowFlashAlert(buttons: FlashMessageButtons.None);

            for (int i = 0; i < OpenFilesBinding.Count; i++)
            {
                alert.Title = $"Saving file {i + 1}/{OpenFilesBinding.Count}";
                alert.Message = $"Saving {OpenFilesBinding[i].Info.FullName} ...";
                await OpenFilesBinding[i].Save(OpenFilesBinding[i].Info.FullName);
            }

            alert.Close();
        }

        private void CloseSelectedFileHandler()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
                CloseFile(openFile);
            else
                log.Warning("No closeable item selected.");
        }

        private void CloseAllFilesHandler()
        {
            foreach (var file in OpenFilesBinding)
                CloseFile(file);
        }

        private void ShowLogViewerHandler()
        {
            // TODO: Unselect the selected item in the OpenFiles ListBox
            if (logCollector != null)
                SelectedItem = new LogViewerViewModel(logCollector);
            else
                log.Warning("Can't create LogViewerViewMOdel because no LogCollector was passed to the MainWindowViewModel.");
        }

        private void CloseFileHandler(IOpenFileViewModel obj)
        {
            CloseFile(obj);
        }

        private async Task SaveFilesHandler(IList<IOpenFileViewModel> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                log.Warning("No items selected.");
            }
            else
            {
                foreach (var file in lst)
                    await SaveFile(file);
            }
        }

        private async Task SaveFilesAsHandler(IList<IOpenFileViewModel> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                log.Warning("No items selected.");
            }
            else
            {
                foreach (var file in lst)
                    await SaveFileAs(file);
            }
        }

        private void CloseFilesHandler(IList<IOpenFileViewModel> lst)
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

        private void OpenFiles(string[] files)
        {
            // TODO: Add cancel button
            var alert = ShowFlashAlert(buttons: FlashMessageButtons.None);

            for (int i = 0; i < files.Length; i++)
            {
                var fileInfo = new FileInfo(files[i]);
                alert.Title = $"Loading file {i + 1}/{files.Length}";
                alert.Message = $"Loading {fileInfo.FullName} ...";

                var vm = TryCreateViewModelForFile(fileInfo);

                if (vm != null)
                    openFilesList.Add(vm);
            }

            alert.Close();
        }

        /// <summary>
        /// Shows SaveFileDialog and saves file when user clicks ok.
        /// </summary>
        /// <param name="openFile"></param>
        /// <returns></returns>
        private async Task SaveFileAs(IOpenFileViewModel openFile)
        {
            var filters = new List<FileDialogFilter> { new FileDialogFilter("GMD Text File", new List<string> { "gmd" }) };
            var name = await framework.SaveFileDialog(openFile.Info.Name, openFile.Info.Extension, filters);

            if (name != null)
                await SaveFile(openFile, name);
        }

        /// <summary>
        /// Checks if passed file CanClose, calls the Close() method on the file, and removes it from the openFilesList.
        /// </summary>
        /// <param name="vm"></param>
        private void CloseFile(IOpenFileViewModel vm)
        {
            if (vm.CanClose && vm.Close())
            {
                SelectedItem = null;
                openFilesList.Remove(vm);
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
        private async Task SaveFile(IOpenFileViewModel file, string? savePath = null)
        {
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
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not save file.");
                ShowFlashAlert("Could not save file!", ex.Message, FlashMessageButtons.Ok);
            }

            alert.Close();
        }

        private IOpenFileViewModel? TryCreateViewModelForFile(FileInfo fileInfo)
        {
            try
            {
                var gmd = new GmdViewModel(fileInfo);
                return gmd;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not load GMD file.");
                ShowFlashAlert("Could not load GMD file!", ex.Message, FlashMessageButtons.Ok);
                return null;
            }
        }

        private FlashMessageViewModel ShowFlashAlert(string title = "", string message = "", FlashMessageButtons buttons = FlashMessageButtons.Ok)
        {
            var alert = new FlashMessageViewModel(title, message, buttons);
            alert.OnClose += (sender, e) => flashMessages.Remove(alert);
            flashMessages.Insert(0, alert);
            return alert;
        }
    }
}
