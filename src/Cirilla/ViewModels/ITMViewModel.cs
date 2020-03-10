using Cirilla.Core.Models;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cirilla.ViewModels
{
    public class ITMViewModel : FileTypeTabItemViewModelBase
    {
        private ITM _context;

        public ITMItemViewModel SelectedItem { get; set; }
        public ObservableCollection<ITMItemViewModel> Items { get; private set; } = new ObservableCollection<ITMItemViewModel>();

        public GMD LinkedGmd { get; private set; }

        public RelayCommand LinkGmdCommand { get; }

        public ITMViewModel(string path) : base(path)
        {
            _context = new ITM(path);

            foreach (var item in _context.Items)
                Items.Add(new ITMItemViewModel(item, this));

            LinkGmdCommand = new RelayCommand(LinkGmd, () => true);
        }

        public override void Save(string path)
        {
            throw new NotImplementedException();
        }

        public override bool CanSave() => false;

        public override void Close()
        {
            // If has unedited changes then show messagebox asking if user wants to save first
        }

        public override bool CanClose() => true;

        public void LinkGmd()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Item Names|item_*.gmd|All GMD files|*.gmd";

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

        public string GetNameForItem(ITMItemViewModel item)
        {
            if (LinkedGmd == null)
                return null;

            if (item.Id * 2 < LinkedGmd.Entries.Count)
                return LinkedGmd.Entries[item.Id * 2].Value;
            else
                return "(not in range)";
        }
    }
}
