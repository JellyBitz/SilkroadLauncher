using System;
using System.Windows;
using System.Windows.Data;
namespace SilkroadLauncher
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToInvisibilityConverter : IValueConverter
    {
        public static BooleanToInvisibilityConverter Instance = new BooleanToInvisibilityConverter();
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Hidden:Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
