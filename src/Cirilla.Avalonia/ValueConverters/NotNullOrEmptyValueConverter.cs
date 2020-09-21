using Avalonia.Data.Converters;
using Serilog;
using System;
using System.Collections;
using System.Globalization;

namespace Cirilla.Avalonia.ValueConverters
{
    public class NotNullOrEmptyValueConverter : IValueConverter
    {
        private readonly static ILogger log = Log.ForContext<NotNullOrEmptyValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (value is string str) return !string.IsNullOrEmpty(str);
            if (value is IList lst) return lst?.Count > 0;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            log.Error("Unsupported call to ConvertBack! Value: {Value}  Target: {TargetType}  Parameter: {Parameter}",
                value, targetType, parameter);
            return null!;
        }
    }
}
