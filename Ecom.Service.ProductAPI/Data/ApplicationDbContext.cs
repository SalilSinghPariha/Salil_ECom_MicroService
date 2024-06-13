using Ecom.Service.ProductAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom.Service.ProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Samosa",
                Price = 15,
                Description = "Akouna Special.",
                ImageUrl = "",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Paneer Tikka",
                Price = 13.99,
                Description = "Akouna Special.",
                ImageUrl = "",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Sweet Pie",
                Price = 10.99,
                Description = "Akouna Special.",
                ImageUrl = "",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Pav Bhaji",
                Price = 15,
                Description = "Akouna Special.",
                ImageUrl = "",
                CategoryName = "Entree"
            });
        }
        public DbSet<Product> Products { get; set; }
    }
}
