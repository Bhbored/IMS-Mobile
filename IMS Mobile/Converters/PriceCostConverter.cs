using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class PriceCostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IMS_Mobile.MVVM.Models.Product product)
            {
                return $"Price: {product.Price} LBP | Cost: {product.Cost} LBP";
            }
            else if(value is IMS_Mobile.MVVM.Models.TransactionProductItem item)
                return $"Price: ${item.Price} LBP | Cost: {item.Cost} LBP";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}