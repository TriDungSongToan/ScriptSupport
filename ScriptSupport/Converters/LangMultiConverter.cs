using System.Windows.Data;
using System.Globalization;

namespace ScriptSupport.Converters
{
    public class LangMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0)
                return "";

            var format = values[0]?.ToString() ?? "";

            if (values.Length == 1)
                return format;

            var args = values.Skip(1).ToArray();

            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format; // fallback an toàn
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
