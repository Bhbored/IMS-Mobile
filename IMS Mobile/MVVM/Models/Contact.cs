using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{

    public class Contact : Entity
    {

        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public double CreditScore{ get; set; }
        public double TotalPurchases { get; set; }
         

    }
}
