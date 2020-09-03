using System.IO;

namespace Cirilla.Avalonia.ViewModels
{
    public interface IFileViewModel
    {
        FileInfo Info { get; }
        bool CanClose { get; }
        bool CanSave { get; }

        bool Close();
        void Save();
    }
}
