using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class CreditScoreColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double creditScore)
            {
                if (creditScore >= 1000)
                    return Colors.LightGreen; // Green for excellent
                else if (creditScore >= 2000)
                    return Colors.Orange; // Orange for good
                else if (creditScore >= 3000)
                    return Colors.Yellow; // Yellow for fair
                else if(creditScore >=4000)
                    return Colors.Red; // Red for poor
            }
            return Colors.Gray; // Gray default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}