using Cirilla.ViewModels;
using System.Windows;

namespace Cirilla.Windows
{
    /// <summary>
    /// Interaction logic for ImportCsvWindow.xaml
    /// </summary>
    public partial class ImportCsvWindow : Window
    {
        public ImportCsvWindow(GMDViewModel gmd)
        {
            InitializeComponent();

            DataContext = new ImportCsvWindowViewModel(gmd, this);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
