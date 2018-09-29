using Cirilla.Core.Models;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cirilla.ViewModels
{
    public class NBSCViewModel : FileTypeTabItemViewModelBase
    {
        private NBSC _context;

        public NBSCItemViewModel SelectedItem { get; set; }
        public ObservableCollection<NBSCItemViewModel> Items { get; private set; } = new ObservableCollection<NBSCItemViewModel>();

        public GMD LinkedGmd { get; private set; }

        public RelayCommand LinkGmdCommand { get; }

        public NBSCViewModel(string path) : base(path)
        {
            _context = new NBSC(path);

            foreach (var item in _context.Items)
                Items.Add(new NBSCItemViewModel(item, this));

            LinkGmdCommand = new RelayCommand(LinkGmd, () => true);
        }

        public override void Save(string path)
        {
            // Copy edited item (from vm) into NBSC context
            for (int i = 0; i < _context.Items.Count; i++)
            {
                _context.Items[i] = Items[i].Item;
            }

            try
            {
                _context.Save(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public override bool CanSave() => true;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;

        public void LinkGmd()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Character Names|chr_names_*.gmd|All GMD files|*.gmd";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    LinkedGmd = new GMD(ofd.FileName);

                    // Force refresh datagrid, NotifyPropertyChanged doesn't work for some reason
                    var temp = Items;
                    Items = null;
                    Items = temp;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        public string GetNameForItem(NBSCItemViewModel item)
        {
            if (LinkedGmd == null)
                return null;

            if (item.Id < LinkedGmd.Entries.Count)
                return LinkedGmd.Entries[item.Id].Value;
            else
                return "(not in range)";
        }
    }
}
