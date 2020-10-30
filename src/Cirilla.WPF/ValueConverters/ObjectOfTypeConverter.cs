using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cirilla.WPF.ValueConverters
{
    class ObjectOfTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                if (value == null)
                    return VisibilityConverter.BoolToVisibility(false);

                if (parameter is Type t)
                    return VisibilityConverter.BoolToVisibility(value.GetType() == t);
            }
            else if (targetType == typeof(bool))
            {
                if (value == null)
                    return false;

                if (parameter is Type t)
                    return value.GetType() == t;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
