using Cirilla.MVVM.ViewModels;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Cirilla.WPF.Views
{
    /// <summary>
    /// Interaction logic for GmdCsvView.xaml
    /// </summary>
    public partial class CsvView : UserControl
    {
        public CsvView()
        {
            InitializeComponent();

            csvEntriesDataGrid.LoadingRow += LoadingRowShowIndexInHeader;
            cmbGmdFile.SelectionChanged += CmbGmdFile_SelectionChanged;
        }

        public CsvViewModel ViewModel => DataContext as CsvViewModel ?? throw new Exception("DataContext is not of type 'GmdViewModel'.");

        private void LoadingRowShowIndexInHeader(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
        }

        private void CmbGmdFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGmdFile.SelectedItem is Button)
            {
                if (ViewModel.OpenGmdFiles.Count > 0)
                    cmbGmdFile.SelectedItem = ViewModel.OpenGmdFiles.First();
                else
                    cmbGmdFile.SelectedItem = null;
            }
        }

        private void OpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.PickGmdFileToLinkCommand.Execute().Subscribe();
        }

        private void AutoSelectFromFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            cmbGmdFile.IsDropDownOpen = false;
            ViewModel.AutoSelectFromFolderCommand.Execute().Subscribe();
        }

        private void AutoSelectFromOpenedFiles_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            cmbGmdFile.IsDropDownOpen = false;
            ViewModel.AutoSelectFromOpenedFilesCommand.Execute().Subscribe();
        }
    }
}
