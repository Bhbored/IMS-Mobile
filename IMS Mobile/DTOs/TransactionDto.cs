using IMS_Mobile.MVVM.Models;
using Supabase.Postgrest.Attributes;
using System;

namespace IMS_Mobile.DTOs
{
    [Table("transaction")]
    public class TransactionDto : BaseDto
    {
        [Column("totalamount")]
        public double totalamount { get; set; }

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("is_paid")]
        public bool IsPaid { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("local_contact_id")]
        public int ContactId { get; set; }

        public static TransactionDto FromModel(Transaction transaction, string currentUserId)
        {
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                Guid.TryParse(currentUserId, out userId);
            }
            return new TransactionDto
            {
                // Don't set Id - let database auto-generate
                LocalId = transaction.Id,
                UserId = userId,
                totalamount = transaction.totalamount,
                Type = transaction.Type,
                IsPaid = transaction.IsPaid,
                CreatedDate = transaction.CreatedDate,
                ContactId = transaction.ContactId
            };
        }

        public Transaction ToModel()
        {
            return new Transaction
            {
                Id = this.LocalId, // Use LocalId for the model ID
                totalamount = this.totalamount,
                Type = this.Type,
                IsPaid = this.IsPaid,
                CreatedDate = this.CreatedDate,
                ContactId = this.ContactId
            };
        }
    }
}