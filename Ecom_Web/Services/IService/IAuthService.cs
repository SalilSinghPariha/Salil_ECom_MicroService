using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto> RegistrationAsync(RegisterRequestDto registerRequestDto);
        Task<ResponseDto> RoleAsync(RegisterRequestDto registerRequestDto);
    }
}
