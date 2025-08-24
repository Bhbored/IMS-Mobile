using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace IMS_Mobile.DTOs
{
    public abstract class BaseDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
    }
}