using Avalonia;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructor for design time preview. Shouldn't be used in your actual code.
        /// </summary>
        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(Window window)
        {
            this.window = window;

            OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFile);
            CloseFileCommand = ReactiveCommand.Create<IFileViewModel>(CloseFile);

            openFilesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out openFilesBinding)
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedItem)
                .Do(x => Debug.WriteLine(x))
                .Select(x => x == null ? "Cirilla" : $"{x.Info.Name} - Cirilla")
                .ToPropertyEx(this, x => x.Title);

            this.WhenAnyValue(x => x.FlashAlert.IsVisible)
                .Select(x => x ? (double)Application.Current.FindResource("ThemeDisabledOpacity")! : 1.0)
                .Do(x => Console.WriteLine(x))
                .ToPropertyEx(this, x => x.ContentOpacity);
        }

        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
        public ReactiveCommand<IFileViewModel, Unit> CloseFileCommand { get; }

        public ReadOnlyObservableCollection<IFileViewModel> OpenFiles => openFilesBinding;

        [Reactive] public IFileViewModel? SelectedItem { get; set; }
        [Reactive] public FlashMessageViewModel FlashAlert { get; set; } = new FlashMessageViewModel();

        [ObservableAsProperty] public string Title { get; set; }
        [ObservableAsProperty] public double ContentOpacity { get; set; }

        private static readonly ILogger logger = Log.ForContext<MainWindowViewModel>();
        private readonly Window window;
        private readonly ReadOnlyObservableCollection<IFileViewModel> openFilesBinding;
        private readonly SourceList<IFileViewModel> openFilesList = new SourceList<IFileViewModel>();

        private async Task OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog { AllowMultiple = true };
            ofd.Filters.Add(new FileDialogFilter
            {
                Name = "GMD Text File",
                Extensions = new List<string> { "gmd" }
            });

            var files = await ofd.ShowAsync(window);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var vm = TryCreateViewModelForFile(fileInfo);

                if (vm != null)
                    openFilesList.Add(vm);
            }
        }

        private void CloseFile(IFileViewModel vm)
        {
            if (vm.Close())
            {
                SelectedItem = null;
                openFilesList.Remove(vm);
            }
        }

        private IFileViewModel? TryCreateViewModelForFile(FileInfo fileInfo)
        {
            try
            {
                var gmd = new GmdViewModel(fileInfo);
                return gmd;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not load GMD file. Error: " + ex.Message);
                return null;
            }
        }
    }
}
