using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using Cirilla.Helpers;

namespace Cirilla.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object SelectedItem { get; set; }
        public int SelectedItemIndex { get; set; }
        public ObservableCollection<FileTypeTabItemViewModelBase> OpenItems { get; set; } = new ObservableCollection<FileTypeTabItemViewModelBase>();

        #region Commands

        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand<FileTypeTabItemViewModelBase> CloseFileCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            SaveFileCommand = new RelayCommand(SaveFile, CanSaveFile);
            CloseFileCommand = new RelayCommand<FileTypeTabItemViewModelBase>(CloseFile, CanCloseFile);
        }

        public void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    FileTypeTabItemViewModelBase item = Utility.GetViewModelForFile(ofd.FileName);
                    OpenItems.Add(item);
                    SelectedItem = item;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        public bool CanOpenFile() => true;

        public void SaveFile()
        {
            if (SelectedItem is FileTypeTabItemViewModelBase item)
                item.Save();
        }

        public bool CanSaveFile() => SelectedItem is FileTypeTabItemViewModelBase;

        public void CloseFile(FileTypeTabItemViewModelBase item)
        {
            item.Close();
            OpenItems.Remove(item);
            SelectedItemIndex--;
        }

        public bool CanCloseFile(FileTypeTabItemViewModelBase item) => item.CanClose();
    }
}
