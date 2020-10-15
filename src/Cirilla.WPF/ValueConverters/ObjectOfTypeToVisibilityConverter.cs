using System;
using System.Globalization;
using System.Windows.Data;

namespace Cirilla.WPF.ValueConverters
{
    class ObjectOfTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return VisibilityConverter.BoolToVisibility(false);

            if (parameter is Type t)
                return VisibilityConverter.BoolToVisibility(value.GetType() == t);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
