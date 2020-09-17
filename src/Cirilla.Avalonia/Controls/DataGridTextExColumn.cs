using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Cirilla.Avalonia.Controls
{
    public class DataGridTextExColumn : DataGridTextColumn
    {
        public static readonly StyledProperty<bool> AcceptsReturnProperty =
            AvaloniaProperty.Register<DataGridTextExColumn, bool>(nameof(AcceptsReturn));

        public bool AcceptsReturn
        {
            get { return GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        protected override IControl GenerateEditingElementDirect(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateEditingElementDirect(cell, dataItem);
            if (element is TextBox textBox)
                textBox.AcceptsReturn = AcceptsReturn;
            return element;
        }

        protected override object PrepareCellForEdit(IControl editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is TextBox textBox)
                textBox.AcceptsReturn = AcceptsReturn;
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        protected override void RefreshCellContent(IControl element, string propertyName)
        {
            if (element is TextBox textBox)
                textBox.AcceptsReturn = AcceptsReturn;
            base.RefreshCellContent(element, propertyName);
        }
    }
}
