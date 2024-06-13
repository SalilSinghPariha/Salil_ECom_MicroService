using Ecom.Service.EmailAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom.Service.EmailAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public DbSet<EmailLoggger> emailLogggers { get; set; }
    }
}
