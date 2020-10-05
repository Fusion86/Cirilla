using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Cirilla.MVVM.ViewModels;

namespace Cirilla.Avalonia.Views
{
    public class GmdEditView : UserControl
    {
        //private DataGrid dataGrid => this.FindControl<DataGrid>("dataGrid");
        private SolidColorBrush invalidMessageBrush = new SolidColorBrush(0x33ffff00);
        private SolidColorBrush emptyKeyBrush = new SolidColorBrush(0x66ff0000);

        public GmdEditView()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is GmdEntryViewModel entry)
            {
                // HACK: Hardcoded values
                if (entry.Key == null)
                    e.Row.Background = emptyKeyBrush;
                else if (entry.Value == "Invalid Message")
                    e.Row.Background = invalidMessageBrush;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
