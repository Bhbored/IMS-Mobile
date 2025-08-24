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

        [Column("contact_id")]
        public int ContactId { get; set; }

        public static TransactionDto FromModel(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
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
                Id = this.Id,
                totalamount = this.totalamount,
                Type = this.Type,
                IsPaid = this.IsPaid,
                CreatedDate = this.CreatedDate,
                ContactId = this.ContactId
            };
        }
    }
}