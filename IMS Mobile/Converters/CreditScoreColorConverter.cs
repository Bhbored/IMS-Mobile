using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class CreditScoreColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double creditScore)
            {
                if (creditScore >= 800)
                    return Color.FromHex("#4CAF50"); // Green for excellent
                else if (creditScore >= 700)
                    return Color.FromHex("#FF9800"); // Orange for good
                else if (creditScore >= 600)
                    return Color.FromHex("#FFC107"); // Yellow for fair
                else
                    return Color.FromHex("#F44336"); // Red for poor
            }
            return Color.FromHex("#9E9E9E"); // Gray default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}