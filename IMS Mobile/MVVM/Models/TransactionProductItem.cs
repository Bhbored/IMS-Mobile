using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public class TransactionProductItem : Entity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public double Cost { get; set; }

        [ForeignKey(typeof(Transaction))]
        public int TransactionId { get; set; }
        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }
        [ManyToOne] public Transaction Transaction { get; set; }
        [ManyToOne] public Product Product { get; set; }
    }
}
