using System;
using System.Globalization;
using System.Windows.Data;

namespace DovizKuru.converters
{
    internal class ChangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double change)
            {
                return change switch
                {
                    > 0 => "Green",
                    < 0 => "Red",
                    _ => "Black"
                };
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();  
        }
    }
}
