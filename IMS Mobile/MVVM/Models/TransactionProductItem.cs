using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public class TransactionProductItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string CategoryName { get; set; }
        public double Cost { get; set; }
    }
}
