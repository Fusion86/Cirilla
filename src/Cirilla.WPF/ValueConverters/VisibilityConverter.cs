using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cirilla.WPF.ValueConverters
{
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new NotSupportedException("Can only convert to 'Visibility' type.");

            if (value is int i)
                value = i != 0;

            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;

            if (value is string str)
                return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
