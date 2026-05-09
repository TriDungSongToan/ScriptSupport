using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return Visibility.Collapsed;

            var str = value as string;
            bool isEmpty = string.IsNullOrWhiteSpace(str);
            if (Inverse) isEmpty = !isEmpty;

            return isEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
