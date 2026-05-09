using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class FontSizeTextConverter : IValueConverter
    {
        // VM → View: double → string hiển thị trên ComboBox
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d % 1 == 0
                    ? ((int)d).ToString()   // 12.0 → "12"
                    : d.ToString("0.#");    // 14.5 → "14.5"
            return "12";
        }

        // View → VM: string user gõ → double
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is not string text)
                return DependencyProperty.UnsetValue;

            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return DependencyProperty.UnsetValue; // giữ nguyên giá trị cũ

            if (double.TryParse(text, NumberStyles.Any,
                CultureInfo.InvariantCulture, out double result))
            {
                // Clamp trong khoảng hợp lệ
                result = Math.Clamp(result, 6.0, 72.0);
                return result;
            }

            return DependencyProperty.UnsetValue; // parse fail → giữ nguyên
        }
    }
}
