using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using JdCat.CatClient.Common;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Enum;

namespace Jiandanmao.Converter
{
    public class TangOrderSourceToStringTypeConverter : IValueConverter
    {
        private static Dictionary<OrderSource, string> dic;
        static TangOrderSourceToStringTypeConverter()
        {
            dic = new Dictionary<OrderSource, string>();
            foreach (OrderSource status in System.Enum.GetValues(typeof(OrderSource)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (OrderSource)value;
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
