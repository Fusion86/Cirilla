namespace Cirilla.MVVM.ViewModels
{
    public class GmdCsvBatchToolViewModel : ViewModelBase, IExplorerItem
    {
        public string Title { get; } = "Batch GMD/CSV Operations";
        public string StatusText { get; } = "";
        public bool CanClose { get; } = true;

        public bool Close()
        {
            return true;
        }
    }
}
