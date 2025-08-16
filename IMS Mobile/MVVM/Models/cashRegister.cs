using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class cashRegister 
    {
       
        public double CashFlow { get; set; }
        public double InventoryValue { get; set; }
        public double TotalSales { get; set; }
        public double TotalCredit { get; set; }
        public double TotalPurchases { get; set; }

    }
}
