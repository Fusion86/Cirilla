using Cirilla.MVVM.Interfaces;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class BatchImportViewModel : ViewModelBase, ISidebarItemViewModel
    {
        public BatchImportViewModel()
        {
            framework = Locator.Current.GetService<IFrameworkService>();

            PickGmdFolderCommand = ReactiveCommand.CreateFromTask(ShowPickGmdFolderDialog);
            PickCsvFolderCommand = ReactiveCommand.CreateFromTask(ShowPickCsvFolderDialog);
        }

        public ReactiveCommand<Unit, Unit> PickGmdFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> PickCsvFolderCommand { get; }

        [Reactive] public string GmdFolder { get; set; } = "";
        [Reactive] public string CsvFolder { get; set; } = "";

        public string Title { get; } = "Batch Import";
        public bool CanClose { get; } = true;

        private readonly IFrameworkService framework;
        private readonly SourceList<GmdChangeSetViewModel> changeSetItems = new SourceList<GmdChangeSetViewModel>();

        public bool Close()
        {
            return true;
        }

        private async Task ShowPickGmdFolderDialog()
        {
            var folder = await framework.OpenFolderDialog();

            if (folder != null)
                GmdFolder = folder;
        }

        private async Task ShowPickCsvFolderDialog()
        {
            var folder = await framework.OpenFolderDialog();

            if (folder != null)
                CsvFolder = folder;
        }
    }
}
