using Cirilla.Properties;
using Cirilla.ViewModels;
using Serilog;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Cirilla
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

            Log.Information("Cirilla v" + Assembly.GetExecutingAssembly().GetName().Version);
            Log.Information("Cirilla.Core v" + Assembly.GetAssembly(typeof(Core.Models.GMD)).GetName().Version);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                    if (File.Exists(file))
                        vm.OpenFile(file);
            }
        }

        private void GMDEntries_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {
            GMDViewModel gmd = (vm.SelectedItem as GMDViewModel);

            if (gmd != null)
            {
                e.NewItem = new GMDEntryViewModel { Index = gmd.Entries.Count, Key = "CIRILLA_KEY_" + gmd.Entries.Count };
            }
        }
    }
}
