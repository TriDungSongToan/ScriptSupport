using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class BoolToGridLengthConverter : IValueConverter
    {
        public GridLength TrueValue { get; set; } = new GridLength(1, GridUnitType.Star);
        public GridLength FalseValue { get; set; } = new GridLength(0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
