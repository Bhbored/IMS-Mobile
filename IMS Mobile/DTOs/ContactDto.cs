using Supabase.Postgrest.Attributes;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.DTOs
{
    [Table("contact")]
    public class ContactDto : BaseDto
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("phone_number")]
        public int PhoneNumber { get; set; }

        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("credit_score")]
        public double CreditScore { get; set; }

        [Column("total_purchases")]
        public double TotalPurchases { get; set; }

        // Factory method for easy conversion
        public static ContactDto FromModel(Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address ?? string.Empty,
                Email = contact.Email ?? string.Empty,
                CreditScore = contact.CreditScore,
                TotalPurchases = contact.TotalPurchases
            };
        }

        public Contact ToModel()
        {
            return new Contact
            {
                Id = this.Id,
                Name = this.Name,
                PhoneNumber = this.PhoneNumber,
                Address = this.Address,
                Email = this.Email,
                CreditScore = this.CreditScore,
                TotalPurchases = this.TotalPurchases
            };
        }
    }
}