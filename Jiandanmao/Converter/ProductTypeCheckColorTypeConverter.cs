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
    public class ProductTypeCheckColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isCheck = (bool)value;
            if (isCheck)
            {
                return "#ffa78c";
            }
            return "#ddd";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
