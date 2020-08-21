using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using Cirilla.Helpers;
using Cirilla.Windows;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;

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
        public RelayCommand OpenOptionsCommand { get; }
        public RelayCommand SaveInWorkingDirectoryCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            SaveFileCommand = new RelayCommand(SaveFile, CanSaveFile);
            CloseFileCommand = new RelayCommand<FileTypeTabItemViewModelBase>(CloseFile, CanCloseFile);
            OpenOptionsCommand = new RelayCommand(OpenOptions, CanOpenOptions);
            SaveInWorkingDirectoryCommand = new RelayCommand(SaveInWorkingDirectory, CanSaveInWorkingDirectory);
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

        public void OpenOptions()
        {
            new OptionsWindow().ShowDialog();
        }

        public bool CanOpenOptions() => true;

        public void SaveInWorkingDirectory()
        {
            if (SelectedItem is FileTypeTabItemViewModelBase item)
            {
                string rel = GetFullRelativePathForGameFile(item.Filepath);
                string path = Path.Combine(Properties.Settings.Default.Config.WorkingDirectoryPath, rel);

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                item.Save(path);
            }
        }

        public bool CanSaveInWorkingDirectory()
        {
            if (SelectedItem is FileTypeTabItemViewModelBase item)
                if (Properties.Settings.Default.Config != null && Properties.Settings.Default.Config.WorkingDirectoryPath != null)
                    return Directory.Exists(Properties.Settings.Default.Config.WorkingDirectoryPath)
                        && GetFullRelativePathForGameFile(item.Filepath) != null;

            return false;
        }

        private string GetFullRelativePathForGameFile(string path)
        {
            // Changes ... into ...     This is bad code, pls don't look at
            // "L:\MHWMods\chunk0\common\text\steam\w_sword_eng.gmd"                                            >   "common\text\steam\w_sword_eng.gmd"
            // "C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\steam\w_sword_eng.gmd"      >   "common\text\steam\w_sword_eng.gmd"

            // Check if we opened a file from inside the working directory
            if (Properties.Settings.Default.Config.WorkingDirectoryPath != null
                && path.Contains(Properties.Settings.Default.Config.WorkingDirectoryPath))
            {
                int len = Properties.Settings.Default.Config.WorkingDirectoryPath.Length;
                return path.Remove(0, len).TrimStart(new[] { '\\' });
            }
            else
            {
                Regex regex = new Regex(@".*chunk\d+\\(.*)");
                Match match = regex.Match(path);

                if (!match.Success) return null;

                return match.Groups[1].Value;
            }
        }
    }
}
