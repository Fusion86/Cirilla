using Avalonia.Data.Converters;
using Serilog;
using System;
using System.Globalization;

namespace Cirilla.Avalonia.ValueConverters
{
    public class CalculatorValueConverter : IValueConverter
    {
        private readonly static ILogger log = Log.ForContext<CalculatorValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double nr && parameter is string p && int.TryParse(p, out var add))
                return nr + add;

            log.Error("Unsupported call to Convert! Value: {Value}  Target: {TargetType}  Parameter: {Parameter}",
                value, targetType, parameter);
            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            log.Error("Unsupported call to ConvertBack! Value: {Value}  Target: {TargetType}  Parameter: {Parameter}",
                value, targetType, parameter);
            return null!;
        }
    }
}
