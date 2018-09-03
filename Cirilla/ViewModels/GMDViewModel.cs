using Cirilla.Core.Models;
using Cirilla.Core.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Serilog;
using Microsoft.Win32;
using System.IO;

namespace Cirilla.ViewModels
{
    public class GMDViewModel : FileTypeTabItemViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Title => Context.Filename;

        public string Filename { get; }
        public GMD Context { get; }

        // Editable stuff
        public ObservableCollection<KeyValueViewModel> HeaderMetadata { get; private set; } = new ObservableCollection<KeyValueViewModel>();
        public ObservableCollection<KeyValueViewModel> Entries { get; private set; } = new ObservableCollection<KeyValueViewModel>();
        public object CurrentlyOpenFile { get; private set; }

        public GMDViewModel(string path)
        {
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
            Filename = path;
        }

        public override void Save()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(Filename);
            if (sfd.ShowDialog() == true)
            {
                Save(sfd.FileName);
            }
        }

        public void Save(string path)
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

        public override bool CanSave() => true;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;
    }
}
