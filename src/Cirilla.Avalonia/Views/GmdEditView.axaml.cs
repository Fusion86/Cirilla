using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Cirilla.Avalonia.Views
{
    public class GmdEditView : UserControl
    {
        public GmdEditView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
