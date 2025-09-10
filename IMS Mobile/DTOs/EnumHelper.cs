using IMS_Mobile.MVVM.Models;
using IMS_Mobile.Converters;

namespace IMS_Mobile.DTOs
{
    public static class EnumHelper
    {
        public static string ThemeToString(Theme theme)
        {
            return theme switch
            {
                Theme.Light => "Light",
                Theme.Dark => "Dark",
                _ => "Light" 
            };
        }

        public static Theme StringToTheme(string themeString)
        {
            return themeString switch
            {
                "Light" => Theme.Light,
                "Dark" => Theme.Dark,
                _ => Theme.Light 
            };
        }

        public static string CurrencyToString(Currency currency)
        {
       
            return currency.ToString();
        }

        public static Currency StringToCurrency(string currencyString)
        {
            if (string.IsNullOrEmpty(currencyString))
                return Currency.USD;

            if (Enum.TryParse<Currency>(currencyString, out Currency result))
            {
                GlobalCurrencyConverter.UpdateCurrency(result);
                return result;
            }
            GlobalCurrencyConverter.UpdateCurrency(Currency.USD);
            return Currency.USD;
        }
    }
}