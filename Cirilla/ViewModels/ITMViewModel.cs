using Cirilla.Core.Models;
using System;
using System.Collections.ObjectModel;

namespace Cirilla.ViewModels
{
    public class ITMViewModel : FileTypeTabItemViewModelBase
    {
        private ITM _context;

        public ITMItemViewModel SelectedItem { get; set; }
        public ObservableCollection<ITMItemViewModel> Items { get; private set; } = new ObservableCollection<ITMItemViewModel>();

        public ITMViewModel(string path) : base(path)
        {
            _context = new ITM(path);

            foreach (var item in _context.Items)
                Items.Add(new ITMItemViewModel(item));
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
    }
}
