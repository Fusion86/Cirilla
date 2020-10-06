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
            OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFileHandler);
            SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileHandler);
            CloseFileCommand = ReactiveCommand.Create<IOpenFileViewModel>(CloseFileHandler);
            ShowLogViewerCommand = ReactiveCommand.Create(ShowLogViewerHandler);

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
                .Select(x => x == null ? "Cirilla" : $"{x.Title} - Cirilla")
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
        public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowLogViewerCommand { get; }
        public ReactiveCommand<IOpenFileViewModel, Unit> CloseFileCommand { get; }

        public ReadOnlyObservableCollection<IOpenFileViewModel> OpenFiles => openFilesBinding;
        public ReadOnlyObservableCollection<FlashMessageViewModel> FlashMessages => flashMessagesBinding;

        [Reactive] public ITitledViewModel? SelectedItem { get; set; }

        [ObservableAsProperty] public string Title { get; }
        [Reactive] public string StatusText { get; set; }
        [ObservableAsProperty] public double ContentOpacity { get; }

        private static readonly ILogger logger = Log.ForContext<MainWindowViewModel>();
        private readonly IFrameworkService framework;
        private readonly LogCollector? logCollector;
        private readonly ReadOnlyObservableCollection<IOpenFileViewModel> openFilesBinding;
        private readonly ReadOnlyObservableCollection<FlashMessageViewModel> flashMessagesBinding;
        private readonly SourceList<IOpenFileViewModel> openFilesList = new SourceList<IOpenFileViewModel>();
        private readonly SourceList<FlashMessageViewModel> flashMessages = new SourceList<FlashMessageViewModel>();

        private static readonly ILogger log = Log.ForContext<MainWindowViewModel>();

        private async Task OpenFileHandler()
        {
            var filters = new List<FileDialogFilter> { new FileDialogFilter("GMD Text File", new List<string> { "gmd" }) };
            var files = await framework.OpenFileDialog(true, filters);

            await Task.Run(() =>
            {
                foreach (var file in files)
                    OpenFile(file);
            });
        }

        private async Task SaveFileHandler()
        {
            if (SelectedItem != null && SelectedItem is IOpenFileViewModel openFile)
            {
                var filters = new List<FileDialogFilter> { new FileDialogFilter("GMD Text File", new List<string> { "gmd" }) };
                var name = await framework.SaveFileDialog(openFile.Info.Name, openFile.Info.Extension, filters);
                if (name == null) return;

                var alert = ShowFlashAlert($"Saving {name} ...", buttons: FlashMessageButtons.None);
                flashMessages.Insert(0, alert);

                try
                {
                    openFile.Save(name);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Could not save file.");
                    ShowFlashAlert("Could not save file!", ex.Message, FlashMessageButtons.Ok);
                }

                alert.Close();
            }
        }

        private void CloseFileHandler(IOpenFileViewModel vm)
        {
            if (vm.Close())
            {
                SelectedItem = null;
                openFilesList.Remove(vm);
            }
        }

        private void ShowLogViewerHandler()
        {
            // TODO: Unselect the selected item in the OpenFiles ListBox
            if (logCollector != null)
                SelectedItem = new LogViewerViewModel(logCollector);
            else
                log.Warning("Can't create LogViewerViewMOdel because no LogCollector was passed to the MainWindowViewModel.");
        }

        private void OpenFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            var alert = ShowFlashAlert($"Loading {fileInfo.FullName} ...", buttons: FlashMessageButtons.None);
            var vm = TryCreateViewModelForFile(fileInfo);

            if (vm != null)
                openFilesList.Add(vm);

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
