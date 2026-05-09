using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                var colors = (parameter as string) ?? "ActiveColor=Blue,InactiveColor=Gray";
                var activeColor = Colors.Blue;
                var inactiveColor = Colors.Gray;

                // Parse parameter nếu cần
                return new SolidColorBrush(isActive ? activeColor : inactiveColor);
            }
            return Brushes.Gray;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
    public class ActiveToBackgroundConverter : IMultiValueConverter
    {
        // values[0] = IsSelected
        // values[1] = ThemeColor (tab active)
        // values[2] = Background (tab inactive)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return Brushes.Transparent;

            bool isActive = values[0] is bool b && b;
            var themeColor = values[1] as SolidColorBrush ?? Brushes.Blue;     // fallback
            var background = values[2] as SolidColorBrush ?? Brushes.Transparent; // fallback

            return isActive ? themeColor : background;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
