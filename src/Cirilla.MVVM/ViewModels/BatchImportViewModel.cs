using System;
using System.Collections.Generic;
using System.Text;

namespace Cirilla.MVVM.ViewModels
{
    public class BatchImportViewModel : ViewModelBase, ISidebarItemViewModel
    {
        public string Title { get; } = "Batch Import";
        public bool CanClose { get; } = true;

        public bool Close()
        {
            return true;
        }
    }
}
