using IMS_Mobile.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.Converters
{
    public class DTSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not Transaction transaction)
                return null;
            var key = string.Empty;
            if (transaction.Type == "buy")
            {
                key = "buyTransactions";
            }
            else if (transaction.Type == "sell" && transaction.IsPaid == true)
            {
                key = "sellTransactions";
            }
            else if (transaction.Type == "sell" && transaction.IsPaid == false)
            {
                key = "sellOnTabTransactions";
            }
          
                Application.Current.Resources.TryGetValue(key, out var template);
            return template as DataTemplate;

        }
    }
}
