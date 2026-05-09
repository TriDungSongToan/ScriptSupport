using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class WidthToVisibilityConverter : IValueConverter
    {
        public double Threshold { get; set; } = 80;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                return width < Threshold ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
