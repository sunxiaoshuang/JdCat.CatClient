
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class ProductFontColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "#555";
            var quantity = int.Parse(value.ToString());
            if (quantity > 0)
            {
                return "#fff";
            }
            return "#555";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
