using Cirilla.MVVM.Services;

namespace Cirilla.MVVM.ViewModels
{
    public class LogViewerViewModel : ViewModelBase, ITitledViewModel
    {
        public string Title { get; } = "Log Viewer";
        public LogCollector LogCollector { get; }

        public LogViewerViewModel(LogCollector logCollector)
        {
            LogCollector = logCollector;
        }
    }
}
