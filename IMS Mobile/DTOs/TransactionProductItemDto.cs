using IMS_Mobile.MVVM.Models;
using Supabase.Postgrest.Attributes;

namespace IMS_Mobile.DTOs
{
    [Table("transaction_product_item")]
    public class TransactionProductItemDto : BaseDto
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("price")]
        public double Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("cost")]
        public double Cost { get; set; }

        [Column("transaction_id")]
        public int TransactionId { get; set; }

        public static TransactionProductItemDto FromModel(TransactionProductItem item)
        {
            return new TransactionProductItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                Cost = item.Cost,
                TransactionId = item.TransactionId
            };
        }

        public TransactionProductItem ToModel()
        {
            return new TransactionProductItem
            {
                Id = this.Id,
                Name = this.Name,
                Price = this.Price,
                Quantity = this.Quantity,
                Cost = this.Cost,
                TransactionId = this.TransactionId
            };
        }
    }
}