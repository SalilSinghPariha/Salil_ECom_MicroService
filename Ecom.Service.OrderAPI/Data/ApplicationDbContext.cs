using Ecom.Service.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom.Service.OrderAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
