using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace IMS_Mobile.Converters
{
    public class PaidStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? Color.FromArgb("#E8F5E9") : Color.FromArgb("#FFF3E0"); // Light green for paid, light orange for unpaid
            }
            return Color.FromArgb("#FFF3E0"); // Default to unpaid color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}