using System.IO;

namespace Cirilla.MVVM.ViewModels
{
    public interface IOpenFileViewModel : ITitledViewModel
    {
        FileInfo Info { get; }
        bool CanClose { get; }
        bool CanSave { get; }

        /// <summary>
        /// Signal the file that you want to close it. Returns <see langword="true"/> when file has been closed.
        /// </summary>
        /// <returns><see langword="true"/> when successfully closed.</returns>
        bool Close();

        void Save(string path);
    }
}
