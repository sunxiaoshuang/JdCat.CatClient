using Jiandanmao.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JdCat.CatClient.Model;

namespace Jiandanmao.Converter
{
    public class TangOrderProductDescriptionTypeConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var remark = values[0] + "";
            var description = values[1] + "";
            var result = string.Empty;
            if (!string.IsNullOrEmpty(remark))
            {
                result += remark;
            }
            if (!string.IsNullOrEmpty(description))
            {
                result += $"（{description}）";
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
