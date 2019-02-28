using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Jiandanmao.Enum;
using Jiandanmao.Extension;

namespace Jiandanmao.Converter
{
    public class OrderStatusToStringValueConverter : IValueConverter
    {
        private static Dictionary<OrderStatus, string> dic;
        static OrderStatusToStringValueConverter()
        {
            dic = new Dictionary<OrderStatus, string>();
            foreach (OrderStatus status in System.Enum.GetValues(typeof(OrderStatus)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (OrderStatus)value;
            if (dic.ContainsKey(status))
            {
                return dic[status];
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
