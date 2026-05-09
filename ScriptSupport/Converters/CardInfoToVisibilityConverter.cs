using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class CardInfoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            var propertyName = parameter?.ToString();
            if (string.IsNullOrWhiteSpace(propertyName)) return Visibility.Collapsed;

            var prop = value.GetType().GetProperty(propertyName);
            if (prop == null) return Visibility.Collapsed;

            var result = prop.GetValue(value);

            return (result is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
    }
}
