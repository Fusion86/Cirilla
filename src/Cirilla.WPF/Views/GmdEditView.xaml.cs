using Cirilla.MVVM.ViewModels;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cirilla.WPF.Views
{
    /// <summary>
    /// Interaction logic for GmdEditView.xaml
    /// </summary>
    public partial class GmdEditView : UserControl
    {
        public GmdEditView()
        {
            InitializeComponent();
        }

        public GmdViewModel ViewModel => DataContext as GmdViewModel ?? throw new Exception("DataContext is not of type 'GmdViewModel'.");
        private static readonly ILogger log = Log.ForContext<GmdEditView>();

        private void DeleteSelectedRows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = dataGrid.SelectedItems.Cast<GmdEntryViewModel>().ToList();
                ViewModel.DeleteRowsCommand.Execute(selectedItems).Subscribe();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Can't cast openFilesListBox.SelectedItems to 'IList<IOpenFileViewModel>'.");
            }
        }
    }
}
