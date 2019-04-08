using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class ButtonColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isCheck = (bool)value;
            if (isCheck)
            {
                return "#862000";
            }
            return "#009789";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
