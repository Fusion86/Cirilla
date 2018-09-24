using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cirilla.Converters
{
    class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            Color converted = Color.FromArgb(color.A, color.R, color.G, color.B);
            return new SolidColorBrush(converted);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public static System.Drawing.Color RGBToColor(string rgb)
        //{
        //    if (rgb.Length > 6)
        //    {
        //        rgb = rgb.Substring(rgb.Length - 6);
        //    }

        //    if (rgb.Length != 6)
        //        throw new ArgumentException("Invalid rgb value given");

        //    int red = 0;
        //    int green = 0;
        //    int blue = 0;

        //    red = System.Convert.ToInt32(rgb.Substring(0, 2), 16);
        //    green = System.Convert.ToInt32(rgb.Substring(2, 2), 16);
        //    blue = System.Convert.ToInt32(rgb.Substring(4, 2), 16);


        //    return System.Drawing.Color.FromArgb(red, green, blue);
        //}

        //public static string ColorToRGB(System.Drawing.Color color)
        //{
        //    string red = color.R.ToString("X2");
        //    string green = color.G.ToString("X2");
        //    string blue = color.B.ToString("X2");
        //    return String.Format("{0}{1}{2}", red, green, blue);
        //}

        //public static Color ColorToColor(System.Drawing.Color color)
        //{
        //    return Color.FromArgb(color.A, color.R, color.G, color.B);
        //}

        //public static System.Drawing.Color ColorToColor(Color color)
        //{
        //    return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        //}
    }
}
