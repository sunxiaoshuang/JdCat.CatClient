﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using JdCat.CatClient.Common;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Enum;

namespace Jiandanmao.Converter
{
    public class TangOrderStatusToStringTypeConverter : IValueConverter
    {
        private static Dictionary<TangOrderStatus, string> dic;
        static TangOrderStatusToStringTypeConverter()
        {
            dic = new Dictionary<TangOrderStatus, string>();
            foreach (TangOrderStatus status in System.Enum.GetValues(typeof(TangOrderStatus)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (TangOrderStatus)value;
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
