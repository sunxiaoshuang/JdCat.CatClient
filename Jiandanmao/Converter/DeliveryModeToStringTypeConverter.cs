using Jiandanmao.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Jiandanmao.Code;
using Jiandanmao.Extension;

namespace Jiandanmao.Converter
{
    public class DeliveryModeToStringTypeConverter : IValueConverter
    {
        private static Dictionary<DeliveryMode, string> dic;
        static DeliveryModeToStringTypeConverter()
        {
            dic = new Dictionary<DeliveryMode, string>();
            foreach (DeliveryMode status in System.Enum.GetValues(typeof(DeliveryMode)))
            {
                dic.Add(status, status.GetDescription());
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (DeliveryMode)value;
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
