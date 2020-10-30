using Cirilla.MVVM.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public interface IOpenFileViewModel : ISidebarItemViewModel
    {
        FileInfo Info { get; }
        bool CanSave { get; }
        IList<FileDialogFilter> SaveFileDialogFilters { get; }

        Task Save(string path);
    }
}
