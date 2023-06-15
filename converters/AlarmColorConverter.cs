using System;
using System.Globalization;
using System.Windows.Data;

namespace DovizKuru.converters
{
    internal class AlarmColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0 ? "Red" : "LightGray";
            }
            return "DarkGray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException();
    }
}
