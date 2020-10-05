
using Cirilla.MVVM.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cirilla.MVVM.Interfaces
{
    public interface IFrameworkService
    {
        Task<string[]> OpenFileDialog(bool allowMultiple = false, List<FileDialogFilter>? filters = null);
        Task<string> SaveFileDialog(string? defaultName = null, string? extension = null, List<FileDialogFilter>? filters = null);
        object? FindResource(string key);
    }
}
