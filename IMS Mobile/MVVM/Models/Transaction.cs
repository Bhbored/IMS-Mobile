using Humanizer;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double totalamount { get; set; }

        public string Type { get; set; } //sell or buy

        public bool IsPaid { get; set; } = false;

        [NotMapped]
        public string Status => IsPaid == true ? "Paid" : "Unpaid";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int ContactId { get; set; } = 0;


        [NotMapped]
        public string HumanDate
        {
            get
            {
                return CreatedDate.Humanize();
            }
        }

        public List<TransactionProductItem> Products { get; set; } = new();
    }

   
}
