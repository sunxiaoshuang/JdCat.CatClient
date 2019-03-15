using JdCat.CatClient.Model.Enum;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class GenderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = (Gender)value;
            switch (obj)
            {
                case Gender.None:
                    return "未知";
                case Gender.Boy:
                    return "男";
                case Gender.Girl:
                    return "女";
                default:
                    break;
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
