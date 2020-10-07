using System;
using System.IO;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdCsvViewModel : ViewModelBase, IOpenFileViewModel
    {
        public GmdCsvViewModel(FileInfo fileInfo)
        {
            Info = fileInfo;
        }

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;

        public bool Close()
        {
            return true;
        }

        public Task Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
