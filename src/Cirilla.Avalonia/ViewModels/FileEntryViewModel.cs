using System.IO;

namespace Cirilla.Avalonia.ViewModels
{
    public class FileEntryViewModel : ViewModelBase, IFileSystemEntry
    {
        public FileEntryViewModel(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public string Name => fileInfo.Name;
        public string FullName => fileInfo.FullName;

        private readonly FileInfo fileInfo;
    }
}
