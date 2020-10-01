using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Cirilla.Avalonia.ViewModels;

namespace Cirilla.Avalonia.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void MenuExit_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: Check for unsaved changes
            Close();
        }
    }
}
