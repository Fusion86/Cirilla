using Cirilla.MVVM.Services;
using Cirilla.MVVM.ViewModels;
using Serilog;
using Serilog.Formatting.Compact;
using System.Windows;

namespace Cirilla.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logCollector = new LogCollector();
            string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] <{SourceContext}> {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.Debug(outputTemplate: outputTemplate)
                .WriteTo.File(new RenderedCompactJsonFormatter(), "log.json")
                .WriteTo.Sink(logCollector)
                .CreateLogger();

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainWindowViewModel(null);
            MainWindow.Show();
        }
    }
}
