using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace IMS_Mobile.Converters
{
    public class PaidStatusToBadgeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF9800");
            }
            return Color.FromArgb("#FF9800"); // Default to unpaid badge color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}