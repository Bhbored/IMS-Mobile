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
        public string? Address { get; set; }=string.Empty;
        public string? Email { get; set; }=string.Empty;
        public double CreditScore{ get; set; }=0.0;
        public double TotalPurchases { get; set; }= 0.0;


    }
}
