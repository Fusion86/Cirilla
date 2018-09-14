using Cirilla.Core.Models;
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

        public NBSCViewModel(string path) : base(path)
        {
            _context = new NBSC(path);

            foreach (var item in _context.Items)
                Items.Add(new NBSCItemViewModel(item));
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
