using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DovizKuru.converters
{
    internal class ChangeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double change)
            {
                return change switch
                {
                    > 0 => PackIconMaterialKind.ArrowUpBold,
                    < 0 => PackIconMaterialKind.ArrowDownBold,
                    _ => PackIconMaterialKind.Minus
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
