using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Converters
{
    public class SellTConv : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double amount)
            {
                var currencySymbol = GlobalCurrencyConverter.GetCurrencySymbol();
                return $"+ {amount:F2} {currencySymbol}";
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
