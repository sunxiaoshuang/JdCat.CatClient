using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Jiandanmao.Enum;
using Jiandanmao.Extension;

namespace Jiandanmao.Converter
{
    public class PrintModelToStringTypeConverter : IValueConverter
    {
        private static Dictionary<PrinterMode, string> dic;
        static PrintModelToStringTypeConverter()
        {
            dic = new Dictionary<PrinterMode, string>();
            foreach (PrinterMode status in System.Enum.GetValues(typeof(PrinterMode)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (PrinterMode)value;
            if (dic.ContainsKey(status))
            {
                return dic[status];
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            switch (str)
            {
                case "一菜一打":
                    return PrinterMode.Food;
                case "一份一打":
                    return PrinterMode.Share;
                case "一单一打":
                    return PrinterMode.Order;
                default:
                    break;
            }
            throw new ArgumentException($"不存在打印机模式[{str}]");
        }
    }
}
