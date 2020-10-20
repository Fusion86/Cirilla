using ReactiveUI.Fody.Helpers;

namespace Cirilla.MVVM.ViewModels
{
    public class ProjectViewModel : ViewModelBase, ITitledViewModel
    {
        public string Title { get; } = "Project";

        [Reactive] public string Name { get; set; } = "Cirilla Project";
        [Reactive] public string SourceDirectory { get; set; } = "Cirilla Project";
        [Reactive] public string ProjectDirectory { get; set; } = "Cirilla Project";
    }
}
