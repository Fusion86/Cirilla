namespace Cirilla.MVVM.ViewModels
{
    public interface IExplorerItem : ITitledViewModel
    {
        string StatusText { get; }
        bool CanClose { get; }

        /// <summary>
        /// Signal the file that you want to close it. Returns <see langword="true"/> when file has been closed.
        /// </summary>
        /// <returns><see langword="true"/> when successfully closed.</returns>
        bool Close();
    }
}
