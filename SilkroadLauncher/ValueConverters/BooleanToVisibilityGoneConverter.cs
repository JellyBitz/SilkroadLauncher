using System;
using System.Windows;
using System.Windows.Data;
namespace SilkroadLauncher
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityGoneConverter : IValueConverter
    {
        public static BooleanToVisibilityGoneConverter Instance = new BooleanToVisibilityGoneConverter();
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed:Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
