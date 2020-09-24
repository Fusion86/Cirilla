using Cirilla.Avalonia.Helpers;
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
        public string FileSizeString => Utility.BytesToSizeString(fileInfo.Length);

        private readonly FileInfo fileInfo;
    }
}
