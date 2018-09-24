using Cirilla.ViewModels;
using System.Windows;

namespace Cirilla.Windows
{
    /// <summary>
    /// Interaction logic for ExportAppearanceWindow.xaml
    /// </summary>
    public partial class ExportAppearanceWindow : Window
    {
        private readonly SaveSlotViewModel _saveSlot;

        public ExportAppearanceWindow(SaveSlotViewModel saveSlot)
        {
            InitializeComponent();

            _saveSlot = saveSlot;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtContent.Text = _saveSlot.GetAppearanceJson();
        }
    }
}
