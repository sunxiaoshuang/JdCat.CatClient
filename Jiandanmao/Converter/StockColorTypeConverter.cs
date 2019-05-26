
using JdCat.CatClient.Model;
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class StockColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         {
            var quantity = (double)value;
            if (quantity > 0) return "AntiqueWhite";
            return "#009789";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
