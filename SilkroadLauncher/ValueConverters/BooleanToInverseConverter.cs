using System;
using System.Windows;
using System.Globalization;
namespace SilkroadLauncher
{
    /// <summary>
    /// A converter that takes in a boolean and returns his inverse value
    /// </summary>
    public class BooleanToInverseConverter : BaseValueConverter<BooleanToInverseConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(),out bool result))
                return !result;
            return false;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
