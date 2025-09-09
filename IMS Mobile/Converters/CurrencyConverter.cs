using System.Globalization;
using IMS_Mobile.MVVM.Models;

namespace IMS_Mobile.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Currency currency)
            {
                return currency switch
                {
                    Currency.USD => "💵 USD - US Dollar",
                    Currency.EUR => "💶 EUR - Euro",
                    Currency.GBP => "💷 GBP - British Pound",
                    Currency.JPY => "💴 JPY - Japanese Yen",
                    Currency.LBP => "💸 LBP - Lebanese Pound",
                    _ => "💵 USD - US Dollar"
                };
            }
            return "💵 USD - US Dollar";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string currencyString)
            {
                return currencyString switch
                {
                    "💵 USD - US Dollar" => Currency.USD,
                    "💶 EUR - Euro" => Currency.EUR,
                    "💷 GBP - British Pound" => Currency.GBP,
                    "💴 JPY - Japanese Yen" => Currency.JPY,
                    "💸 LBP - Lebanese Pound" => Currency.LBP,
                    _ => Currency.USD
                };
            }
            return Currency.USD;
        }
    }
}
