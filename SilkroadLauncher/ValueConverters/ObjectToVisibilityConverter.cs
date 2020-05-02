using System;
using System.Windows;
using System.Windows.Data;
namespace SilkroadLauncher
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public static ObjectToVisibilityConverter Instance = new ObjectToVisibilityConverter();
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Boolean filter
            if (value is bool v)
                return v;

            return value == null ? Visibility.Hidden : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
