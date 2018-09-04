using System.ComponentModel;
using System.IO;

namespace Cirilla.ViewModels
{
    public abstract class FileTypeTabItemViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Path.GetFileName(Filepath);
        public string Filepath { get; }

        public virtual bool HasUnsavedChanges => false;

        public virtual void Save() { }
        public virtual bool CanSave() => true;

        public virtual void Close() { }
        public virtual bool CanClose() => true;

        protected FileTypeTabItemViewModelBase(string path)
        {
            Filepath = path;
        }
    }
}
