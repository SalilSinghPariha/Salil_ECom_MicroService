using Ecom_ShoppingCartAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom_ShoppingCartAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }
        public DbSet<CartHeader> cartHeaders { get; set; }
        public DbSet<CartDetails> cartDetails { get; set; }
    }
}
