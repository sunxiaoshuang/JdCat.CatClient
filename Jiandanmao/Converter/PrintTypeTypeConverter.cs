using Jiandanmao.Code;
using Jiandanmao.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class PrintTypeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (int)value;
            if(type == 1)
            {
                return "前台打印机";
            }
            else if(type == 2)
            {
                return "后厨打印机";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            switch (str)
            {
                case "前台打印机":
                    return 1;
                case "后厨打印机":
                    return 2;
                default:
                    break;
            }
            throw new ArgumentException($"不存在打印机类型[{str}]");
        }
    }
}
