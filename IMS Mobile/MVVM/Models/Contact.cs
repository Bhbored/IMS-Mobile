using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Contact : Entity
    {
        private double calculatedCredit;

        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string? Address { get; set; }=string.Empty;
        public string? Email { get; set; }=string.Empty;
        public double CreditScore { get; set; } = 0.00;
        public double TotalPurchases { get; set; }= 0.00;

        //public double CalculatedCredit
        //{
        //    get => calculatedCredit;
        //    set
        //    {
        //        calculatedCredit = value;
        //    }
        //}


        //public void CalculateCredit()
        //{
        //    var contacttrans = App.TransactionRepository
        //        .GetItems()
        //        .Where(x=> x.ContactId == Id)
        //        .ToList();
        //    var contactcredit = contacttrans
        //        .Where(x => x.IsPaid == false);
        //    var creditscore = contactcredit.Sum(x=>x.totalamount);

        //}
    }
}
