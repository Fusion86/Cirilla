using Ookii.Dialogs.Wpf;
using System.Windows;

namespace Cirilla.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void WorkingDirectory_Browse(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();

            if (fbd.ShowDialog() == true)
            {
                Properties.Settings.Default.Config.WorkingDirectoryPath = fbd.SelectedPath;
            }
        }

        private void ExtractedFiles_Browse(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();

            if (fbd.ShowDialog() == true)
            {
                Properties.Settings.Default.Config.ExtractedFilesPath = fbd.SelectedPath;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
