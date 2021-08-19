using Cirilla.MVVM.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public interface IExplorerFileItem : IExplorerItem
    {
        FileInfo Info { get; }
        bool CanSave { get; }
        IList<FileDialogFilter> SaveFileDialogFilters { get; }

        Task Save(string path);
    }
}
