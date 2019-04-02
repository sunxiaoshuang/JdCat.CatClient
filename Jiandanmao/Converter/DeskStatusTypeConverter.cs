using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class DeskStatusTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return "#e64a19";
            }
            return "#eee";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
