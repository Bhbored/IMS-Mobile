using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS_Mobile.MVVM.Models;
using Microsoft.EntityFrameworkCore;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.DB
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath;

        public AppDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.OwnsMany(t => t.Products, p =>
                {
                    p.ToJson();
                });
            });

            // Optional: Customize mappings here if needed
            // modelBuilder.Entity<Product>().ToTable("Products");
            //    modelBuilder.Entity<Product>()
            //.Property(p => p.Name)
            //.HasColumnName("ProductName"); // DB column = "ProductName", not "Name"

            //    modelBuilder.Entity<Product>()
            //        .Property(p => p.Price)
            //        .HasDefaultValue(0.0m);
        }
    }
}
