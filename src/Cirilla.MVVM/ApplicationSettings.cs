using ReactiveUI.Fody.Helpers;

namespace Cirilla.MVVM
{
    public class ApplicationSettings
    {
        // Not the worst way to do this, but we'll get some problems once we actually want to save/load the settings.
        public static ApplicationSettings Current { get; } = new();

        [Reactive] public bool ShowGmdTextPreview { get; set; } = false;
        [Reactive] public bool SmoothScroll { get; set; } = false;
    }
}
