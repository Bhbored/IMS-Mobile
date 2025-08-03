using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Converters
{
    public class TransactionColorConv : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
           if(value is string type)
           {
               return type switch
               {
                   "sell" => "#2dfca3", 
                   "buy" => "#fc2d84", 
                   _ => "#FFFFFF" 
               };
                
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
