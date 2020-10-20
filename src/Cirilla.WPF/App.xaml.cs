using Cirilla.MVVM.Services;
using Cirilla.MVVM.ViewModels;
using Cirilla.WPF.Windows;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;
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

            var log = Log.ForContext<App>();

            log.Information("Cirilla version " + Assembly.GetExecutingAssembly().GetName().Version);
            log.Information("Cirilla.Core version " + Assembly.GetAssembly(typeof(Core.Models.GMD))?.GetName().Version ?? "(not found)");

            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainWindowViewModel(new FrameworkService(), logCollector);
            MainWindow.Show();
        }
    }
}
