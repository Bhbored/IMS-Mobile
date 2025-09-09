using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.Converters
{
    public class BuyTConv : IValueConverter
    {

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double amount)
            {
                var currencySymbol = GlobalCurrencyConverter.GetCurrencySymbol();
                return $"- {amount:F2} {currencySymbol}";
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
