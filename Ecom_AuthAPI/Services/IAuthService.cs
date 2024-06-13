using Ecom_AuthAPI.Models.Dto;

namespace Ecom_AuthAPI.Services
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> login(LoginRequestDto loginRequestDto);

        Task<bool> AssignRole(string email, string roleName);
    }
}
