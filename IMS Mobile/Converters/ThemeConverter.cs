using System.Globalization;
using IMS_Mobile.MVVM.Models;

namespace IMS_Mobile.Converters
{
    public class ThemeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Theme theme)
            {
                return theme switch
                {
                    Theme.Light => "☀️ Light Theme",
                    Theme.Dark => "🌙 Dark Theme",
                    _ => "☀️ Light Theme"
                };
            }
            return "☀️ Light Theme";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string themeString)
            {
                return themeString switch
                {
                    "☀️ Light Theme" => Theme.Light,
                    "🌙 Dark Theme" => Theme.Dark,
                    _ => Theme.Light
                };
            }
            return Theme.Light;
        }
    }
}
