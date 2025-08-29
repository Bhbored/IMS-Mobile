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

        public static TransactionProductItemDto FromModel(TransactionProductItem item, string currentUserId)
        {
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                Guid.TryParse(currentUserId, out userId);
            }
            return new TransactionProductItemDto
            {
                // Don't set Id - let database auto-generate
                LocalId = item.Id,
                UserId = userId,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                Cost = item.Cost,
                TransactionId = item.TransactionId // This is the local transaction ID
            };
        }

        public TransactionProductItem ToModel()
        {
            return new TransactionProductItem
            {
                Id = this.LocalId, // Use LocalId for the model ID
                Name = this.Name,
                Price = this.Price,
                Quantity = this.Quantity,
                Cost = this.Cost,
                TransactionId = this.TransactionId
            };
        }
    }
}