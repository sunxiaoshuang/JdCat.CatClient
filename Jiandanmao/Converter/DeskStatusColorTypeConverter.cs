using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Jiandanmao.Entity;
using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class DeskStatusColorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return "White";
            }
            return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
