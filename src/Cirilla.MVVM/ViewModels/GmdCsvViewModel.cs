using Cirilla.Core.Helpers;
using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Cirilla.MVVM.Models;
using CsvHelper;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
    public class GmdCsvViewModel : ViewModelBase, IOpenFileViewModel
    {
        public GmdCsvViewModel(FileInfo fileInfo, MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            Info = fileInfo;
            ReloadCsvCommand = ReactiveCommand.Create(() => LoadCsv(Info.FullName));
            PickGmdFileToLinkCommand = ReactiveCommand.CreateFromTask(PickGmdFileToLinkHandler);
            AutoSelectFromFolderCommand = ReactiveCommand.CreateFromTask(AutoSelectFromFolderHandler);
            AutoSelectFromOpenedFilesCommand = ReactiveCommand.Create(AutoSelectFromOpenedFilesHandler);

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

            ReloadCsvCommand.Execute().Subscribe();
        }

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;

        public ReactiveCommand<Unit, Unit> ReloadCsvCommand { get; }
        public ReactiveCommand<Unit, Unit> PickGmdFileToLinkCommand { get; }
        public ReactiveCommand<Unit, Unit> AutoSelectFromFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> AutoSelectFromOpenedFilesCommand { get; }

        [Reactive] public GmdViewModel? SelectedGmdFile { get; set; }

        public ReadOnlyObservableCollection<StringKeyValuePair> CsvEntries => csvEntriesBinding;
        public ReadOnlyObservableCollection<GmdViewModel> OpenGmdFiles => openGmdFilesBinding;

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ReadOnlyObservableCollection<StringKeyValuePair> csvEntriesBinding;
        private readonly ReadOnlyObservableCollection<GmdViewModel> openGmdFilesBinding;
        private readonly SourceCache<StringKeyValuePair, string> csvEntries = new SourceCache<StringKeyValuePair, string>(x => x.Key);

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
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (TextReader tr = new StreamReader(fs, ExEncoding.UTF8))
            using (CsvReader csv = new CsvReader(tr, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = ";";
                csv.Configuration.AllowComments = true; // Uses # to identify comments

                var values = csv.GetRecords<StringKeyValuePair>();

                csvEntries.Edit(x =>
                {
                    x.Load(values);
                });
            }
        }

        private async Task PickGmdFileToLinkHandler()
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

        private async Task AutoSelectFromFolderHandler()
        {
            var folder = await mainWindowViewModel.framework.OpenFolderDialog();
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

        private void AutoSelectFromOpenedFilesHandler()
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
