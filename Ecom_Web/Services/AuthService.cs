using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;

namespace Ecom_Web.Services
{
    public class AuthService : IAuthService

    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {

            _baseService = baseService;

        }
        public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthApiBase + "/api/AuthAPI/login"
            }, jwtBearer: false);
        }

        public async Task<ResponseDto> RegistrationAsync(RegisterRequestDto registerRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = SD.AuthApiBase + "/api/AuthAPI/Register"
            },jwtBearer:false);
        }

        public async Task<ResponseDto> RoleAsync(RegisterRequestDto registerRequestDto)
        {
         return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = SD.AuthApiBase + "/api/AuthAPI/AssignRole"
         });
        }
    }
}
