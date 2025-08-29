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
        public string PhoneNumber { get; set; } = string.Empty;

        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("credit_score")]
        public double CreditScore { get; set; }

        [Column("total_purchases")]
        public double TotalPurchases { get; set; }

        public static ContactDto FromModel(Contact contact, string currentUserId)
        {
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                Guid.TryParse(currentUserId, out userId);
            }
            return new ContactDto
            {
                LocalId = contact.Id,
                UserId = userId,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber.ToString(),
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
                Id = this.LocalId, 
                Name = this.Name,
                PhoneNumber = int.TryParse(this.PhoneNumber, out int phone) ? phone : 0,
                Address = this.Address,
                Email = this.Email,
                CreditScore = this.CreditScore,
                TotalPurchases = this.TotalPurchases
            };
        }
    }
}