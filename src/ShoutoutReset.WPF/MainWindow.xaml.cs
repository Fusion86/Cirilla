using System;
using System.Linq;
using System.Windows;

namespace ShoutoutReset.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectAllNone_Click(object sender, RoutedEventArgs e)
        {
            if (LstShoutouts.SelectedItems.Count == 0)
            {
                LstShoutouts.SelectAll();
            }
            else
            {
                LstShoutouts.UnselectAll();
            }
        }

        private void ResetSelected_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                var selectedShoutouts = LstShoutouts.SelectedItems.OfType<ShoutoutViewModel>().ToList();
                vm.ResetSelectedShoutouts.Execute(selectedShoutouts).Subscribe();
            }
        }
    }
}
