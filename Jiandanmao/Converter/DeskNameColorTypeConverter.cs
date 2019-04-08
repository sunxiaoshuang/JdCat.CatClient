using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class DeskNameColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return "White";
            }
            return "#009789";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
