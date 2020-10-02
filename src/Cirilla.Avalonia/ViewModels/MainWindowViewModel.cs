﻿using Avalonia;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
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
            SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFile);
            CloseFileCommand = ReactiveCommand.Create<IOpenFileViewModel>(CloseFile);

            openFilesList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out openFilesBinding)
                .Subscribe();

            flashMessages.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out flashMessagesBinding)
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedItem)
                .Do(x => Debug.WriteLine(x))
                .Select(x => x == null ? "Cirilla" : $"{x.Info.Name} - Cirilla")
                .ToPropertyEx(this, x => x.Title);

            var shouldShowFlashAlert = flashMessages.CountChanged
                .Select(x => x > 0);

            shouldShowFlashAlert
                .Select(x => x ? (double)Application.Current.FindResource("ThemeDisabledOpacity")! : 1.0)
                .Do(x => Console.WriteLine(x))
                .ToPropertyEx(this, x => x.ContentOpacity);
        }

        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
        public ReactiveCommand<IOpenFileViewModel, Unit> CloseFileCommand { get; }

        public ReadOnlyObservableCollection<IOpenFileViewModel> OpenFiles => openFilesBinding;
        public ReadOnlyObservableCollection<FlashMessageViewModel> FlashMessages => flashMessagesBinding;

        [Reactive] public IOpenFileViewModel? SelectedItem { get; set; }

        [ObservableAsProperty] public string Title { get; set; }
        [ObservableAsProperty] public double ContentOpacity { get; set; }

        private static readonly ILogger logger = Log.ForContext<MainWindowViewModel>();
        private readonly Window window;
        private readonly ReadOnlyObservableCollection<IOpenFileViewModel> openFilesBinding;
        private readonly ReadOnlyObservableCollection<FlashMessageViewModel> flashMessagesBinding;
        private readonly SourceList<IOpenFileViewModel> openFilesList = new SourceList<IOpenFileViewModel>();
        private readonly SourceList<FlashMessageViewModel> flashMessages = new SourceList<FlashMessageViewModel>();

        private static readonly ILogger log = Log.ForContext<MainWindowViewModel>();

        private async Task OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog { AllowMultiple = true };
            ofd.Filters.Add(new FileDialogFilter
            {
                Name = "GMD Text File",
                Extensions = new List<string> { "gmd" }
            });

            var files = await ofd.ShowAsync(window);

            await Task.Run(() =>
            {
                foreach (var file in files)
                    OpenFile(file);
            });
        }

        private async Task SaveFile()
        {
            if (SelectedItem == null) return;

            SaveFileDialog sfd = new SaveFileDialog()
            {
                InitialFileName = SelectedItem.Info.Name,
                DefaultExtension = SelectedItem.Info.Extension,
            };

            sfd.Filters.Add(new FileDialogFilter
            {
                Name = "GMD Text File",
                Extensions = new List<string> { "gmd" }
            });

            var name = await sfd.ShowAsync(window);
            if (name == null) return;

            var alert = ShowFlashAlert($"Saving {name} ...", buttons: FlashMessageButtons.None);
            flashMessages.Insert(0, alert);

            // TODO: Try catch
            SelectedItem.Save(name);

            alert.Close();
        }

        private void CloseFile(IOpenFileViewModel vm)
        {
            if (vm.Close())
            {
                SelectedItem = null;
                openFilesList.Remove(vm);
            }
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

        private void OpenFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            var alert = ShowFlashAlert($"Loading {fileInfo.FullName} ...", buttons: FlashMessageButtons.None);
            var vm = TryCreateViewModelForFile(fileInfo);

            if (vm != null)
                openFilesList.Add(vm);

            alert.Close();
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
