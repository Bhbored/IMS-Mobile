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
                _currentCurrency = Currency.USD; 
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
                Currency.USD => "$",        // US Dollar
                Currency.EUR => "€",        // Euro
                Currency.GBP => "£",        // British Pound
                Currency.JPY => "¥",        // Japanese Yen
                Currency.LBP => "L.L",      // Lebanese Pound
                Currency.CAD => "CA$",      // Canadian Dollar
                Currency.AUD => "A$",       // Australian Dollar
                Currency.CHF => "Fr",       // Swiss Franc
                Currency.CNY => "¥",        // Chinese Yuan
                Currency.SEK => "kr",       // Swedish Krona
                Currency.NOK => "kr",       // Norwegian Krone
                Currency.DKK => "kr",       // Danish Krone
                Currency.PLN => "zł",       // Polish Zloty
                Currency.CZK => "Kč",       // Czech Koruna
                Currency.HUF => "Ft",       // Hungarian Forint
                Currency.RON => "lei",      // Romanian Leu
                Currency.BGN => "лв",       // Bulgarian Lev
                Currency.HRK => "kn",       // Croatian Kuna
                Currency.RSD => "дин",      // Serbian Dinar
                Currency.MKD => "ден",      // Macedonian Denar
                Currency.BAM => "KM",       // Bosnia and Herzegovina Convertible Mark
                Currency.ALL => "L",        // Albanian Lek
                Currency.MNT => "₮",        // Mongolian Tugrik
                Currency.KRW => "₩",        // South Korean Won
                Currency.SGD => "S$",       // Singapore Dollar
                Currency.HKD => "HK$",      // Hong Kong Dollar
                Currency.TWD => "NT$",      // New Taiwan Dollar
                Currency.THB => "฿",        // Thai Baht
                Currency.MYR => "RM",       // Malaysian Ringgit
                Currency.IDR => "Rp",       // Indonesian Rupiah
                Currency.PHP => "₱",        // Philippine Peso
                Currency.VND => "₫",        // Vietnamese Dong
                Currency.INR => "₹",        // Indian Rupee
                Currency.PKR => "₨",        // Pakistani Rupee
                Currency.BDT => "৳",        // Bangladeshi Taka
                Currency.LKR => "Rs",       // Sri Lankan Rupee
                Currency.NPR => "₨",        // Nepalese Rupee
                Currency.AFN => "؋",        // Afghan Afghani
                Currency.IRR => "﷼",        // Iranian Rial
                Currency.IQD => "ع.د",      // Iraqi Dinar
                Currency.JOD => "د.ا",      // Jordanian Dinar
                Currency.KWD => "د.ك",      // Kuwaiti Dinar
                Currency.BHD => ".د.ب",     // Bahraini Dinar
                Currency.QAR => "ر.ق",      // Qatari Rial
                Currency.AED => "د.إ",      // UAE Dirham
                Currency.OMR => "ر.ع.",     // Omani Rial
                Currency.SAR => "ر.س",      // Saudi Riyal
                Currency.YER => "﷼",        // Yemeni Rial
                Currency.EGP => "ج.م",      // Egyptian Pound
                Currency.MAD => "د.م.",     // Moroccan Dirham
                Currency.TND => "د.ت",      // Tunisian Dinar
                Currency.DZD => "د.ج",      // Algerian Dinar
                Currency.LYD => "ل.د",      // Libyan Dinar
                Currency.SDG => "ج.س.",     // Sudanese Pound
                Currency.ETB => "Br",       // Ethiopian Birr
                Currency.KES => "KSh",      // Kenyan Shilling
                Currency.UGX => "USh",      // Ugandan Shilling
                Currency.TZS => "TSh",      // Tanzanian Shilling
                Currency.ZAR => "R",        // South African Rand
                Currency.NGN => "₦",        // Nigerian Naira
                Currency.GHS => "GH₵",      // Ghanaian Cedi
                Currency.XOF => "CFA",      // West African CFA Franc
                Currency.XAF => "FCFA",     // Central African CFA Franc
                Currency.BRL => "R$",       // Brazilian Real
                Currency.ARS => "$",        // Argentine Peso
                Currency.CLP => "$",        // Chilean Peso
                Currency.COP => "$",        // Colombian Peso
                Currency.MXN => "$",        // Mexican Peso
                Currency.PEN => "S/.",      // Peruvian Sol
                Currency.UYU => "$U",       // Uruguayan Peso
                Currency.VES => "Bs.S",     // Venezuelan Bolívar Soberano
                Currency.RUB => "₽",        // Russian Ruble
                Currency.UAH => "₴",        // Ukrainian Hryvnia
                Currency.BYN => "Br",       // Belarusian Ruble
                Currency.KZT => "₸",        // Kazakhstani Tenge
                Currency.UZS => "сум",      // Uzbekistani Som
                Currency.KGS => "с",        // Kyrgyzstani Som
                Currency.TJS => "SM",       // Tajikistani Somoni
                Currency.TMT => "m",        // Turkmenistani Manat
                Currency.AZN => "₼",        // Azerbaijani Manat
                Currency.AMD => "֏",        // Armenian Dram
                Currency.GEL => "₾",        // Georgian Lari
                Currency.TRY => "₺",        // Turkish Lira
                Currency.JMD => "J$",       // Jamaican Dollar
                Currency.BBD => "Bds$",     // Barbadian Dollar
                Currency.BZD => "BZ$",      // Belize Dollar
                Currency.XCD => "EC$",      // East Caribbean Dollar
                Currency.TTD => "TT$",      // Trinidad and Tobago Dollar
                Currency.GYD => "G$",       // Guyanese Dollar
                Currency.SRD => "$",        // Surinamese Dollar
                Currency.FKP => "£",        // Falkland Islands Pound
                Currency.NZD => "NZ$",      // New Zealand Dollar
                Currency.FJD => "FJ$",      // Fijian Dollar
                Currency.PGK => "K",        // Papua New Guinean Kina
                Currency.SBD => "SI$",      // Solomon Islands Dollar
                Currency.VUV => "VT",       // Vanuatu Vatu
                Currency.WST => "WS$",      // Samoan Tala
                Currency.TOP => "T$",       // Tongan Pa'anga
                Currency.XPF => "₣",        // CFP Franc
                _ => "$"                    // Default to USD symbol
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
