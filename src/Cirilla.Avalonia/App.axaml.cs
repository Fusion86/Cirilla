using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cirilla.Avalonia.Services;
using Cirilla.Avalonia.ViewModels;
using Cirilla.Avalonia.Views;
using Serilog;
using Serilog.Formatting.Compact;
using Splat;

namespace Cirilla.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var logCollector = new LogCollector();
            string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] <{SourceContext}> {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.Debug(outputTemplate: outputTemplate)
                .WriteTo.File(new RenderedCompactJsonFormatter(), "log.json")
                .WriteTo.Sink(logCollector)
                .CreateLogger();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = new MainWindowViewModel(desktop.MainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
