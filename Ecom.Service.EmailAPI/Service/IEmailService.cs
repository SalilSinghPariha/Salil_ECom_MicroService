using Ecom.Service.EmailAPI.Model;

namespace Ecom.Service.EmailAPI.Service
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDTO);
        Task EmailUserAndLog(string email);
    }
}
