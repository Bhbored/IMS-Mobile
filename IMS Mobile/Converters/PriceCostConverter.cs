using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class PriceCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IMS_Mobile.MVVM.Models.Product product)
            {
                return $"Price: ${product.Price:F2} | Cost: ${product.Cost:F2}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}