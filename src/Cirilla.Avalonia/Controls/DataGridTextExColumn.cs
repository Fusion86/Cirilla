using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Cirilla.Avalonia.Controls
{
    public class DataGridTextExColumn : DataGridTextColumn
    {
        public static readonly StyledProperty<bool> AcceptsReturnProperty =
            AvaloniaProperty.Register<DataGridTextExColumn, bool>(nameof(AcceptsReturn));

        public static readonly StyledProperty<bool> ForceCrlfProperty =
            AvaloniaProperty.Register<DataGridTextExColumn, bool>(nameof(ForceCrlf));

        public bool AcceptsReturn
        {
            get { return GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public bool ForceCrlf
        {
            get { return GetValue(ForceCrlfProperty); }
            set { SetValue(ForceCrlfProperty, value); }
        }

        public string NewLine => ForceCrlf ? "\r\n" : Environment.NewLine;

        protected override IControl GenerateEditingElementDirect(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateEditingElementDirect(cell, dataItem);
            if (element is TextBox textBox)
            {
                textBox.NewLine = NewLine;
                textBox.AcceptsReturn = AcceptsReturn;
            }
            return element;
        }

        protected override IControl GenerateElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateElement(cell, dataItem);
            if (element is TextBlock textBlock)
            {
                // TODO: Hide blocky character
            }
            return element;
        }

        protected override object PrepareCellForEdit(IControl editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is TextBox textBox)
            {
                textBox.NewLine = NewLine;
                textBox.AcceptsReturn = AcceptsReturn;
            }
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        protected override void RefreshCellContent(IControl element, string propertyName)
        {
            if (element is TextBox textBox)
            {
                textBox.NewLine = NewLine;
                textBox.AcceptsReturn = AcceptsReturn;
            }
            base.RefreshCellContent(element, propertyName);
        }
    }
}
