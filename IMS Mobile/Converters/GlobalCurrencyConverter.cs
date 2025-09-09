using System.Globalization;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.DB;

namespace IMS_Mobile.Converters
{
    public class GlobalCurrencyConverter : IValueConverter
    {
        private static UserPreferencesRepository? _userPreferencesRepository;
        private static Currency _currentCurrency = Currency.USD;

        public static void Initialize(UserPreferencesRepository userPreferencesRepository)
        {
            _userPreferencesRepository = userPreferencesRepository;
            LoadCurrentCurrency();
        }

        private static void LoadCurrentCurrency()
        {
            try
            {
                if (_userPreferencesRepository != null)
                {
                    var preferences = _userPreferencesRepository.GetUserPreferences();
                    _currentCurrency = preferences.Currency;
                }
            }
            catch
            {
                _currentCurrency = Currency.USD; // Default fallback
            }
        }

        public static void UpdateCurrency(Currency newCurrency)
        {
            _currentCurrency = newCurrency;
        }

        public static string GetCurrencySymbol()
        {
            return _currentCurrency switch
            {
                Currency.USD => "USD",
                Currency.EUR => "EUR",
                Currency.GBP => "GBP",
                Currency.JPY => "JPY",
                Currency.LBP => "LBP",
                Currency.CAD => "CAD",
                Currency.AUD => "AUD",
                Currency.CHF => "CHF",
                Currency.CNY => "CNY",
                Currency.SEK => "SEK",
                Currency.NOK => "NOK",
                Currency.DKK => "DKK",
                Currency.PLN => "PLN",
                Currency.CZK => "CZK",
                Currency.HUF => "HUF",
                Currency.RON => "RON",
                Currency.BGN => "BGN",
                Currency.HRK => "HRK",
                Currency.RSD => "RSD",
                Currency.MKD => "MKD",
                Currency.BAM => "BAM",
                Currency.ALL => "ALL",
                Currency.MNT => "MNT",
                Currency.KRW => "KRW",
                Currency.SGD => "SGD",
                Currency.HKD => "HKD",
                Currency.TWD => "TWD",
                Currency.THB => "THB",
                Currency.MYR => "MYR",
                Currency.IDR => "IDR",
                Currency.PHP => "PHP",
                Currency.VND => "VND",
                Currency.INR => "INR",
                Currency.PKR => "PKR",
                Currency.BDT => "BDT",
                Currency.LKR => "LKR",
                Currency.NPR => "NPR",
                Currency.AFN => "AFN",
                Currency.IRR => "IRR",
                Currency.IQD => "IQD",
                Currency.JOD => "JOD",
                Currency.KWD => "KWD",
                Currency.BHD => "BHD",
                Currency.QAR => "QAR",
                Currency.AED => "AED",
                Currency.OMR => "OMR",
                Currency.SAR => "SAR",
                Currency.YER => "YER",
                Currency.EGP => "EGP",
                Currency.MAD => "MAD",
                Currency.TND => "TND",
                Currency.DZD => "DZD",
                Currency.LYD => "LYD",
                Currency.SDG => "SDG",
                Currency.ETB => "ETB",
                Currency.KES => "KES",
                Currency.UGX => "UGX",
                Currency.TZS => "TZS",
                Currency.ZAR => "ZAR",
                Currency.NGN => "NGN",
                Currency.GHS => "GHS",
                Currency.XOF => "XOF",
                Currency.XAF => "XAF",
                Currency.BRL => "BRL",
                Currency.ARS => "ARS",
                Currency.CLP => "CLP",
                Currency.COP => "COP",
                Currency.MXN => "MXN",
                Currency.PEN => "PEN",
                Currency.UYU => "UYU",
                Currency.VES => "VES",
                Currency.RUB => "RUB",
                Currency.UAH => "UAH",
                Currency.BYN => "BYN",
                Currency.KZT => "KZT",
                Currency.UZS => "UZS",
                Currency.KGS => "KGS",
                Currency.TJS => "TJS",
                Currency.TMT => "TMT",
                Currency.AZN => "AZN",
                Currency.AMD => "AMD",
                Currency.GEL => "GEL",
                Currency.TRY => "TRY",
                Currency.JMD => "JMD",
                Currency.BBD => "BBD",
                Currency.BZD => "BZD",
                Currency.XCD => "XCD",
                Currency.TTD => "TTD",
                Currency.GYD => "GYD",
                Currency.SRD => "SRD",
                Currency.FKP => "FKP",
                Currency.NZD => "NZD",
                Currency.FJD => "FJD",
                Currency.PGK => "PGK",
                Currency.SBD => "SBD",
                Currency.VUV => "VUV",
                Currency.WST => "WST",
                Currency.TOP => "TOP",
                Currency.XPF => "XPF",
                _ => "USD"
            };
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double amount)
            {
                var currencySymbol = GetCurrencySymbol();
                return $"{amount:F2} {currencySymbol}";
            }
            return "0.00 USD";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Extract numeric value from currency string
                var parts = stringValue.Split(' ');
                if (parts.Length > 0 && double.TryParse(parts[0], out double result))
                {
                    return result;
                }
            }
            return 0.0;
        }
    }
}
