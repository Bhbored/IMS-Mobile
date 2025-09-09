using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class PriceCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currencySymbol = GlobalCurrencyConverter.GetCurrencySymbol();

            if (value is IMS_Mobile.MVVM.Models.Product product)
            {
                return $"Price: {product.Price:F2} {currencySymbol} | Cost: {product.Cost:F2} {currencySymbol}";
            }
            else if (value is IMS_Mobile.MVVM.Models.TransactionProductItem item)
                return $"Price: {item.Price:F2} {currencySymbol} | Cost: {item.Cost:F2} {currencySymbol}";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}