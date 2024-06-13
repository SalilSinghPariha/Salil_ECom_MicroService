using Ecom_AuthAPI.Models;

namespace Ecom_AuthAPI.Services
{
    public interface IJwtTokenGenerator
    {
        //passing role while generating token
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string>roles);
    }
}
