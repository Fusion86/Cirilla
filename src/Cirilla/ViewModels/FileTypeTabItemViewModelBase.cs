using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Cirilla.ViewModels
{
    public abstract class FileTypeTabItemViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title => Path.GetFileName(Filepath);
        public string Filepath { get; }

        public virtual bool HasUnsavedChanges => false;

        public virtual void Save()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(Filepath);
            if (sfd.ShowDialog() == true)
            {
                Save(sfd.FileName);
            }
        }

        public virtual void Save(string path) { }
        public virtual bool CanSave() => true;

        public virtual void Close() { }
        public virtual bool CanClose() => true;

        public ObservableCollection<FrameworkElement> ExtraMenuItems { get; } = new ObservableCollection<FrameworkElement>();

        protected FileTypeTabItemViewModelBase(string path)
        {
            Filepath = path;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
