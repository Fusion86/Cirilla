using AdonisUI.Controls;
using Cirilla.MVVM.ViewModels;
using Serilog;
using System;
using System.Linq;
using System.Windows;

namespace Cirilla.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (sender, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.SetSelectedExplorerItems = (vms) =>
                    {
                        openFilesListBox.SelectedItems.Clear();
                        foreach (var vm in vms)
                            openFilesListBox.SelectedItems.Add(vm);
                    };
                }
            };
        }

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel ?? throw new Exception("DataContext is not of type 'MainWindowViewModel'.");
        private static readonly ILogger log = Log.ForContext<MainWindow>();

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Check for unsaved changes
            Close();
        }

        private void SaveSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = openFilesListBox.SelectedItems.OfType<IExplorerFileItem>().ToList();
                ViewModel.SaveFilesCommand.Execute(selectedItems).Subscribe();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Can't cast openFilesListBox.SelectedItems to 'IList<IOpenFileViewModel>'.");
            }
        }

        private void SaveSelectedFilesAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = openFilesListBox.SelectedItems.OfType<IExplorerFileItem>().ToList();
                ViewModel.SaveFilesAsCommand.Execute(selectedItems).Subscribe();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Can't cast openFilesListBox.SelectedItems to 'IList<IOpenFileViewModel>'.");
            }
        }

        private void CloseSelectedFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = openFilesListBox.SelectedItems.OfType<IExplorerItem>().ToList();
                ViewModel.CloseItemsCommand.Execute(selectedItems).Subscribe();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Can't cast openFilesListBox.SelectedItems to 'IList<IOpenFileViewModel>'.");
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ViewModel.OpenFiles(files);
            }
        }

        private void openFilesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ITitledViewModel vm)
                ViewModel.ContentViewModel = vm;
        }

        private void showLogButton_Click(object sender, RoutedEventArgs e)
        {
            openFilesListBox.SelectedItems.Clear();
            ViewModel.ShowLogViewerCommand.Execute().Subscribe();
        }
    }
}
