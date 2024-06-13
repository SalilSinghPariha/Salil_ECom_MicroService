using Ecom.Services.CoupanAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom.Services.CoupanAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupan>().HasData( new Coupan
            { 
                CoupanId=1,
                CoupanCode="10OFF",
                DiscountAmount=10,
                MinAmount=20
            }
                );

            modelBuilder.Entity<Coupan>().HasData(new Coupan
            {
                CoupanId = 2,
                CoupanCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40
            }
                );
        }
        public DbSet<Coupan> coupans { get; set; }
    }
}
