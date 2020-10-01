using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using Cirilla.Helpers;
using Serilog;

namespace Cirilla.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object SelectedItem { get; set; }
        public int SelectedItemIndex { get; set; }
        public ObservableCollection<FileTypeTabItemViewModelBase> OpenItems { get; set; } = new ObservableCollection<FileTypeTabItemViewModelBase>();

        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand<FileTypeTabItemViewModelBase> CloseFileCommand { get; }
        public RelayCommand OpenOptionsCommand { get; }
        public RelayCommand SaveInWorkingDirectoryCommand { get; }

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
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
                foreach (var file in ofd.FileNames)
                    OpenFile(file);
        }

        public void OpenFile(string path)
        {
            try
            {
                FileTypeTabItemViewModelBase item = Utility.GetViewModelForFile(path);
                OpenItems.Add(item);
                SelectedItem = item;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public bool CanOpenFile() => true;

        public void SaveFile()
        {
            if (SelectedItem is FileTypeTabItemViewModelBase item)
                item.Save();
        }

        public bool CanSaveFile()
        {
            var item = SelectedItem as FileTypeTabItemViewModelBase;

            if (item != null)
                return item.CanSave();
            else
                return false;
        }

        public void CloseFile(FileTypeTabItemViewModelBase item)
        {
            if (SelectedItem == item)
                SelectedItemIndex--;

            item.Close();
            OpenItems.Remove(item);
        }

        public bool CanCloseFile(FileTypeTabItemViewModelBase item) => item.CanClose();
    }
}
