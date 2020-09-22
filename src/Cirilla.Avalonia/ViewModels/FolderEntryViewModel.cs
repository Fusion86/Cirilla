using System.Collections.ObjectModel;
using System.IO;

namespace Cirilla.Avalonia.ViewModels
{
    public class FolderEntryViewModel : ViewModelBase, IFileSystemEntry
    {
        public FolderEntryViewModel(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }

        public string Name => directoryInfo.Name;
        public string FullName => directoryInfo.FullName;

        public ObservableCollection<IFileSystemEntry> Items { get; } = new ObservableCollection<IFileSystemEntry>();

        private readonly DirectoryInfo directoryInfo;
    }
}
