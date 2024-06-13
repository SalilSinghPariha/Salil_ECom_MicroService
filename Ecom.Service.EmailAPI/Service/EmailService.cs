using Ecom.Service.EmailAPI.Data;
using Ecom.Service.EmailAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ecom.Service.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;

        public EmailService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
           _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
           StringBuilder message = new StringBuilder();

            message.AppendLine("<br/> Cart Email Requested");
            message.AppendLine("<br/> Cart total" + cartDTO.cartHeaderDTO.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach (var item in cartDTO.cartDetailDTOs)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.ProductDTO.Name +" x " + item.count);
                message.AppendLine("<li/>");
            }
            message.AppendLine("<ul/>");

            await LogAndEmail(message.ToString(),cartDTO.cartHeaderDTO.Email);
        }

        public Task EmailUserAndLog(string email)
        {
            string message = "User Registration Succesull. <br/> Email:" + email;
            return LogAndEmail(message, "parihar@akouna.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
            {
            try 
            {
                EmailLoggger emailLoggger = new()
                {
                    Email=email,
                    EmailSent=DateTime.Now,
                    message=message,                   
                };

                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.emailLogggers.AddAsync(emailLoggger);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            { 
                return false;

            }

        }
    }
}
