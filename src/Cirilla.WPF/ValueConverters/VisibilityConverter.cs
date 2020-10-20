using System;
using System.Collections;
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
                return BoolToVisibility(i != 0);

            if (value is IList list)
                return BoolToVisibility(list.Count > 0);

            if (value is bool b)
                return BoolToVisibility(b);

            if (value is string str)
                return BoolToVisibility(!string.IsNullOrWhiteSpace(str));

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        internal static Visibility BoolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
