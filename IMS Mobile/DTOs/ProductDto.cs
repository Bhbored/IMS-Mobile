using IMS_Mobile.MVVM.Models;
using Supabase.Postgrest.Attributes;
using System;

namespace IMS_Mobile.DTOs
{
    [Table("product")]
    public class ProductDto : BaseDto
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("price")]
        public double Price { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("cost")]
        public double Cost { get; set; }

        [Column("stock")]
        public int stock { get; set; }

        public static ProductDto FromModel(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CreatedDate = product.CreatedDate,
                Cost = product.Cost,
                stock = product.stock
            };
        }

        public Product ToModel()
        {
            return new Product
            {
                Id = this.Id,
                Name = this.Name,
                Price = this.Price,
                CreatedDate = this.CreatedDate,
                Cost = this.Cost,
                stock = this.stock
            };
        }
    }
}