using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class PrintStateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (int)value;
            if (state == 1)
            {
                return "正常";
            }
            else if (state == 2)
            {
                return "停用";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            switch (str)
            {
                case "正常":
                    return 1;
                case "停用":
                    return 2;
                default:
                    break;
            }
            throw new ArgumentException($"不存在打印机状态[{str}]");
        }
    }
}
