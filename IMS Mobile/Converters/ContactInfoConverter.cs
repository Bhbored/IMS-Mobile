using System.Globalization;

namespace IMS_Mobile.Converters
{
    public class ContactInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IMS_Mobile.MVVM.Models.Contact contact)
            {
                return $"Phone: {contact.PhoneNumber} |  Total: {contact.TotalPurchases} LBP";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}