using SQLite;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using PrimaryKeyAttribute = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;

namespace IMS_Mobile.DTOs
{
    public abstract class BaseDto : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("local_id")]
        public int LocalId { get; set; }

        // Stores Supabase auth user id (uuid) as string
        [Column("user_id")]
        public Guid UserId { get; set; }
    }
}