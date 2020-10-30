using AdonisUI.Controls;
using Cirilla.MVVM.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
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
                var selectedItems = openFilesListBox.SelectedItems.Cast<IOpenFileViewModel>().ToList();
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
                var selectedItems = openFilesListBox.SelectedItems.Cast<IOpenFileViewModel>().ToList();
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
                var selectedItems = openFilesListBox.SelectedItems.Cast<IOpenFileViewModel>().ToList();
                ViewModel.CloseFilesCommand.Execute(selectedItems).Subscribe();
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
    }
}
