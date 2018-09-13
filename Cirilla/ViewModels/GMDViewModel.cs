using Cirilla.Core.Models;
using Cirilla.Core.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using Serilog;

namespace Cirilla.ViewModels
{
    public class GMDViewModel : FileTypeTabItemViewModelBase
    {
        private GMD _context;

        // Editable stuff
        public ObservableCollection<KeyValueViewModel> HeaderMetadata { get; private set; } = new ObservableCollection<KeyValueViewModel>();
        public ObservableCollection<KeyValueViewModel> Entries { get; private set; } = new ObservableCollection<KeyValueViewModel>();

        public GMDViewModel(string path) : base(path)
        {
            _context = new GMD(path);

            // Header metadata
            HeaderMetadata.Add(new KeyValueViewModel("Version", "0x" + _context.Header.Version.ToHexString(), EditCondition.Never, EditCondition.Never));
            HeaderMetadata.Add(new KeyValueViewModel("Language", _context.Header.Language.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
            HeaderMetadata.Add(new KeyValueViewModel("KeyCount", _context.Header.KeyCount.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
            HeaderMetadata.Add(new KeyValueViewModel("StringCount", _context.Header.StringCount.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
            HeaderMetadata.Add(new KeyValueViewModel("KeyBlockSize", _context.Header.KeyBlockSize.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
            HeaderMetadata.Add(new KeyValueViewModel("StringBlockSize", _context.Header.StringBlockSize.ToString(), EditCondition.Never, EditCondition.UnsafeOnly));
            HeaderMetadata.Add(new KeyValueViewModel("FilenameLength", _context.Header.FilenameLength.ToString(), EditCondition.Never, EditCondition.Never));
            HeaderMetadata.Add(new KeyValueViewModel("Filename", _context.Filename, EditCondition.Never, EditCondition.Never));

            // Entries
            for (int i = 0; i < _context.Header.StringCount; i++)
            {
                Entries.Add(new KeyValueViewModel(
                    _context.Entries[i].GetType() == typeof(GMD_Entry) ? ((GMD_Entry)_context.Entries[i]).Key : "",
                    _context.Entries[i].Value,
                    EditCondition.UnsafeOnly,
                    EditCondition.Always
                    ));
            }
        }

        public override void Save(string path)
        {
            int updatedStrings = 0;

            for (int i = 0; i < _context.Entries.Count; i++)
            {
                if (_context.Entries[i].Value != Entries[i].Value)
                {
                    _context.Entries[i].Value = Entries[i].Value;
                    updatedStrings++;
                }
            }

            Log.Information($"Updated {updatedStrings} strings");

            try
            {
                _context.Save(path);
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
