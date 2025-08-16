using IMS_Mobile.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Converters
{
    public class DTSelector2 : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not TransactionProductItem transaction)
                return null;
            var key = string.Empty;
            if (transaction.Cost == 0)
            {
                key = "SellTransactionDetails";
            }
            else
            {
                key = "BuyTransactionDetails";
            }

            Application.Current.Resources.TryGetValue(key, out var template);
            return template as DataTemplate;

        }
    }
}
