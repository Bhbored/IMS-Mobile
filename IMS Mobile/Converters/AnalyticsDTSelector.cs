using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS_Mobile.MVVM.Models;

namespace IMS_Mobile.Converters
{
    public class AnalyticsDTSelector : DataTemplateSelector
    {
        public DataTemplate BuyTransactionTemplate { get; set; }
        public DataTemplate SellTransactionTemplate { get; set; }
        public DataTemplate SellOnTabTransactionTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Transaction transaction)
            {
                if (transaction.Type == "buy")
                {
                    return BuyTransactionTemplate;
                }
                else if (transaction.Type == "sell" && transaction.IsPaid)
                {
                    return SellTransactionTemplate;
                }
                else if (transaction.Type == "sell" && !transaction.IsPaid)
                {
                    return SellOnTabTransactionTemplate;
                }
            }

            return SellTransactionTemplate;
        }
    }
}
