
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class NumberTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (double)value;
            if (number == 0) return 0;
            if (number > 0) return 1;
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
