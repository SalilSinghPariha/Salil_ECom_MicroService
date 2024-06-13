using Ecom_AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Ecom.Services.AuthAPI.Data
{
    // now Db context ; we nee dto create for applicationuser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
            
        }

        // DbSet for applicationuser and .net core is string enough to identify it and add newly
        // added column in identity table 
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}
