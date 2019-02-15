using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Jiandanmao.Code;
using Jiandanmao.Model;

namespace Jiandanmao.Converter
{
    public class OrderProductTotalToStringTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = (OrderProduct)value;
            return "￥ " + double.Parse(obj.Price + "").ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
