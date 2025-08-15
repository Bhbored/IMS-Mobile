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
    public class TransactionProductItem : Entity
    {
        private int quantity = 1;
        private double price;
        private double cost;
        private double totalCost;

        public string Name { get; set; }
        public double Price { get => price; set => price = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public double Cost { get => cost; set => cost = value; }
        [Ignore]
        public double TotalCost
        {
            get => totalCost;
            set
            {
                totalCost = quantity * cost;
            }
        }
        public double TotalPrice
        {
            get => totalCost;
            set
            {
                totalCost = quantity * price;
            }
        }

        [ForeignKey(typeof(Transaction))]
        public int TransactionId { get; set; }
        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }
        [ManyToOne] public Transaction Transaction { get; set; }
        [ManyToOne] public Product Product { get; set; }
    }
}
