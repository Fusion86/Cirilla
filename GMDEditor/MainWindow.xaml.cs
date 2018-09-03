using Serilog;
using System.Reflection;
using System.Windows;

namespace GMDEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Setup logging
            var sink = new InMemorySink(tbLog);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(sink)
                .CreateLogger();

            Log.Information("GMDEditor v" + Assembly.GetExecutingAssembly().GetName().Version);
            Log.Information("Cirilla.Core v" + Assembly.GetAssembly(typeof(Cirilla.Core.Models.GMD)).GetName().Version);
        }
    }
}
