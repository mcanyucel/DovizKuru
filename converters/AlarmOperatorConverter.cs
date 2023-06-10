using System;
using System.Globalization;
using System.Windows.Data;

namespace DovizKuru.converters
{
    internal class AlarmOperatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is models.AlarmOperator alarmOperator)
            {
                return alarmOperator switch
                {
                    models.AlarmOperator.LessThan => "<",
                    models.AlarmOperator.GreaterThan => ">",
                    _ => throw new InvalidOperationException(),
                };
            }
            else
                throw new ArgumentException("Value is not an AlarmOperator");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException();
    }
}
