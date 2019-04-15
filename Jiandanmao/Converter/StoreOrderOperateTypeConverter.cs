using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;

using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jiandanmao.Converter
{
    public class StoreOrderOperateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value.ToString();
            var mode = parameter.ToString();
            if (val == mode)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
