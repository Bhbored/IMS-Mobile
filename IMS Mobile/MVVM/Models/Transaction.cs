using Humanizer;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Transaction : Entity
    {
     
        public double totalamount { get; set; }

        public string Type { get; set; } //sell or buy

        public bool IsPaid { get; set; } = false;

        [Ignore]
        public string Status => IsPaid == true ? "Paid" : "Unpaid";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int ContactId { get; set; } = 0;

        [Ignore]
        public string HumanDate
        {
            get
            {
                return CreatedDate.Humanize();
            }
        }

        [Ignore]
        public string FormattedDate => CreatedDate.ToString("dd/MM/yyyy");

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionProductItem> Products { get; set; } = new();
    }
}
