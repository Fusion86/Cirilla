using Cirilla.Core.Models;
using Cirilla.MVVM.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class SaveDataViewModel : ViewModelBase, IExplorerFileItem
    {
        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;
        public string StatusText => Info.FullName;

        private SaveData savedata;
        private readonly MainWindowViewModel mainWindowViewModel;

        public SaveDataViewModel(FileInfo fileInfo, MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            Info = fileInfo;
            savedata = new(fileInfo.FullName);
        }

        public IList<FileDialogFilter> SaveFileDialogFilters { get; } = new[] { FileDialogFilter.SaveData };

        public bool Close()
        {
            return true;
        }

        public async Task Save(string path)
        {
            await Task.Run(() => savedata.Save(path));
        }
    }
}
