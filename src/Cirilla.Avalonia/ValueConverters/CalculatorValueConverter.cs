using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Cirilla.Avalonia.ValueConverters
{
    class CalculatorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double nr && parameter is string p && int.TryParse(p, out var add))
                return nr + add;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
