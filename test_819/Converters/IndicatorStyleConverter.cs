// Converters/IndicatorStyleConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace test_819.Converters
{
    public class IndicatorStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                // 返回激活状态的样式名称
                return "ActiveIndicatorStyle";
            }
            // 返回默认状态的样式名称
            return "IndicatorStyle";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}