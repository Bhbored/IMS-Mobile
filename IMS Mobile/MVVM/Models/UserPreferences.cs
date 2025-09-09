using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public enum Theme
    {
        Light,
        Dark
    }

    public enum Currency
    {
        USD,    // US Dollar
        EUR,    // Euro
        GBP,    // British Pound
        JPY,    // Japanese Yen
        LBP,    // Lebanese Pound
        CAD,    // Canadian Dollar
        AUD,    // Australian Dollar
        CHF,    // Swiss Franc
        CNY,    // Chinese Yuan
        SEK,    // Swedish Krona
        NOK,    // Norwegian Krone
        DKK,    // Danish Krone
        PLN,    // Polish Zloty
        CZK,    // Czech Koruna
        HUF,    // Hungarian Forint
        RON,    // Romanian Leu
        BGN,    // Bulgarian Lev
        HRK,    // Croatian Kuna
        RSD,    // Serbian Dinar
        MKD,    // Macedonian Denar
        BAM,    // Bosnia and Herzegovina Mark
        ALL,    // Albanian Lek
        MNT,    // Mongolian Tugrik
        KRW,    // South Korean Won
        SGD,    // Singapore Dollar
        HKD,    // Hong Kong Dollar
        TWD,    // Taiwan Dollar
        THB,    // Thai Baht
        MYR,    // Malaysian Ringgit
        IDR,    // Indonesian Rupiah
        PHP,    // Philippine Peso
        VND,    // Vietnamese Dong
        INR,    // Indian Rupee
        PKR,    // Pakistani Rupee
        BDT,    // Bangladeshi Taka
        LKR,    // Sri Lankan Rupee
        NPR,    // Nepalese Rupee
        AFN,    // Afghan Afghani
        IRR,    // Iranian Rial
        IQD,    // Iraqi Dinar
        JOD,    // Jordanian Dinar
        KWD,    // Kuwaiti Dinar
        BHD,    // Bahraini Dinar
        QAR,    // Qatari Riyal
        AED,    // UAE Dirham
        OMR,    // Omani Rial
        SAR,    // Saudi Riyal
        YER,    // Yemeni Rial
        EGP,    // Egyptian Pound
        MAD,    // Moroccan Dirham
        TND,    // Tunisian Dinar
        DZD,    // Algerian Dinar
        LYD,    // Libyan Dinar
        SDG,    // Sudanese Pound
        ETB,    // Ethiopian Birr
        KES,    // Kenyan Shilling
        UGX,    // Ugandan Shilling
        TZS,    // Tanzanian Shilling
        ZAR,    // South African Rand
        NGN,    // Nigerian Naira
        GHS,    // Ghanaian Cedi
        XOF,    // West African CFA Franc
        XAF,    // Central African CFA Franc
        BRL,    // Brazilian Real
        ARS,    // Argentine Peso
        CLP,    // Chilean Peso
        COP,    // Colombian Peso
        MXN,    // Mexican Peso
        PEN,    // Peruvian Sol
        UYU,    // Uruguayan Peso
        VES,    // Venezuelan Bolivar
        RUB,    // Russian Ruble
        UAH,    // Ukrainian Hryvnia
        BYN,    // Belarusian Ruble
        KZT,    // Kazakhstani Tenge
        UZS,    // Uzbekistani Som
        KGS,    // Kyrgyzstani Som
        TJS,    // Tajikistani Somoni
        TMT,    // Turkmenistani Manat
        AZN,    // Azerbaijani Manat
        AMD,    // Armenian Dram
        GEL,    // Georgian Lari
        TRY,    // Turkish Lira
        JMD,    // Jamaican Dollar
        BBD,    // Barbadian Dollar
        BZD,    // Belize Dollar
        XCD,    // East Caribbean Dollar
        TTD,    // Trinidad and Tobago Dollar
        GYD,    // Guyanese Dollar
        SRD,    // Surinamese Dollar
        FKP,    // Falkland Islands Pound
        NZD,    // New Zealand Dollar
        FJD,    // Fijian Dollar
        PGK,    // Papua New Guinean Kina
        SBD,    // Solomon Islands Dollar
        VUV,    // Vanuatu Vatu
        WST,    // Samoan Tala
        TOP,    // Tongan Pa'anga
        XPF     // CFP Franc
    }

    public class UserPreferences : Entity
    {
        public Theme Theme { get; set; } = Theme.Light;
        public Currency Currency { get; set; } = Currency.USD;
        public string DisplayName { get; set; } = "";
        public string Avatar { get; set; } = "avatar1.png";
        public string UserEmail { get; set; } = "";
    }
}
