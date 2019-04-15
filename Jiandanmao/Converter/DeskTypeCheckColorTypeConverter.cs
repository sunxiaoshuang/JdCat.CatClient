using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class DeskTypeCheckColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isCheck = (bool)value;
            if (isCheck)
            {
                return "#00786a";
            }
            return "#81cbc5";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
