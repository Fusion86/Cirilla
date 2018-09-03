using Cirilla.Core.Models;
using Cirilla.Core.Extensions;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Serilog;

namespace GMDEditor.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentlyOpenFile { get; private set; }
        public string SelectedFileText => CurrentlyOpenFile != null ? "Currently editing: " + CurrentlyOpenFile : "";

        public GMD Context { get; private set; }

        // Editable stuff
        public ObservableCollection<KeyValueViewModel> HeaderMetadata { get; private set; } = new ObservableCollection<KeyValueViewModel>();
        public ObservableCollection<KeyValueViewModel> Entries { get; private set; } = new ObservableCollection<KeyValueViewModel>();

        #region Commands

        public RelayCommand OpenGMDCommand { get; }
        public RelayCommand SaveGMDCommand { get; }
        public RelayCommand SaveAsGMDCommand { get; }
        public RelayCommand CloseGMDCommand { get; }

        #endregion

        #region Options

        public bool AllowUnsafeEditing { get; set; } = false;

        #endregion

        public MainWindowViewModel()
        {
            OpenGMDCommand = new RelayCommand(OpenGMD, CanOpenGMD);
            SaveGMDCommand = new RelayCommand(null, CanSaveGMD);
            SaveAsGMDCommand = new RelayCommand(SaveAsGMD, CanSaveAsGMD);
            CloseGMDCommand = new RelayCommand(CloseGMD, CanCloseGMD);
        }

        public void OpenGMD()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == true)
                OpenGMD(ofd.FileName);
        }

        public void OpenGMD(string path)
        {
            try
            {
                CloseGMD();
                Context = GMD.Load(path);

                if (Context.Header.StringCount != Context.Header.KeyCount)
                    throw new Exception("StringCount is not equal to KeyCount, this is currently not supported!");

                // Header metadata
                HeaderMetadata.Add(new KeyValueViewModel("Version", "0x" + Context.Header.Version.ToHexString(), EditCondition.Never, EditCondition.Never));
                HeaderMetadata.Add(new KeyValueViewModel("Language", Context.Header.Language.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
                HeaderMetadata.Add(new KeyValueViewModel("KeyCount", Context.Header.KeyCount.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
                HeaderMetadata.Add(new KeyValueViewModel("StringCount", Context.Header.StringCount.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
                HeaderMetadata.Add(new KeyValueViewModel("KeyBlockSize", Context.Header.KeyBlockSize.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
                HeaderMetadata.Add(new KeyValueViewModel("StringBlockSize", Context.Header.StringBlockSize.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
                HeaderMetadata.Add(new KeyValueViewModel("FilenameLength", Context.Header.FilenameLength.ToString(), EditCondition.Never, EditCondition.Never));
                HeaderMetadata.Add(new KeyValueViewModel("Filename", Context.Filename, EditCondition.Never, EditCondition.Never));

                // Entries
                for (int i = 0; i < Context.Header.StringCount; i++)
                {
                    Entries.Add(new KeyValueViewModel(
                        Context.Keys[i],
                        Context.Strings[i],
                        EditCondition.UnsafeOnly,
                        EditCondition.Always
                        ));
                }

                // Only set after it opened successfully
                CurrentlyOpenFile = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public bool CanOpenGMD() => true;

        public void CloseGMD()
        {
            HeaderMetadata.Clear();
            Entries.Clear();
            Context = null;
            CurrentlyOpenFile = null;
        }

        public bool CanCloseGMD() => Context != null;

        public void SaveAsGMD()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(CurrentlyOpenFile);
            if (sfd.ShowDialog() == true)
            {
                SaveGMD(sfd.FileName);
            }
        }

        public void SaveGMD(string path)
        {
            int updatedStrings = 0;
            
            for (int i = 0; i < Context.Header.StringCount; i++)
            {
                if (Context.Strings[i] != Entries[i].Value)
                {
                    Context.Strings[i] = Entries[i].Value;
                    updatedStrings++;
                }
            }

            Log.Information($"Updated {updatedStrings} strings");

            try
            {
                Context.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public bool CanSaveAsGMD() => Context != null;
        public bool CanSaveGMD() => false;
    }
}
