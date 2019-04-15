using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Jiandanmao.Enum;
using JdCat.CatClient.Common;
using JdCat.CatClient.Model.Enum;

namespace Jiandanmao.Converter
{
    public class OrderCategoryToStringTypeConverter : IValueConverter
    {
        private static Dictionary<OrderCategory, string> dic;
        static OrderCategoryToStringTypeConverter()
        {
            dic = new Dictionary<OrderCategory, string>();
            foreach (OrderCategory status in System.Enum.GetValues(typeof(OrderCategory)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (OrderCategory)value;
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
