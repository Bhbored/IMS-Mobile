using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace IMS_Mobile.Converters
{
    public class PaidStatusToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? Color.FromArgb("#C8E6C9") : Color.FromArgb("#FFE0B2"); // Green border for paid, orange border for unpaid
            }
            return Color.FromArgb("#FFE0B2"); // Default to unpaid border color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}