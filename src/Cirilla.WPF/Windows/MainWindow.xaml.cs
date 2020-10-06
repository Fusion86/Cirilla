using AdonisUI.Controls;
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

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Check for unsaved changes
            Close();
        }
    }
}
