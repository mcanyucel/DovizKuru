using System;
using System.Globalization;
using System.Windows.Data;

namespace DovizKuru.converters
{
    internal class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            else
                throw new ArgumentException("Value must be boolean", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            else
                throw new ArgumentException("Value must be boolean", nameof(value));
        }
    }
}
