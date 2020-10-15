
using Cirilla.MVVM.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cirilla.MVVM.Interfaces
{
    public interface IFrameworkService
    {
        Task<string[]> OpenFileDialog(bool allowMultiple = false, IList<FileDialogFilter>? filters = null);
        Task<string?> OpenFolderDialog();
        Task<string?> SaveFileDialog(string? defaultName = null, string? extension = null, IList<FileDialogFilter>? filters = null);
        object? FindResource(string key);
    }
}
