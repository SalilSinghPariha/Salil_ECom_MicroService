using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface IBaseService
    {
       Task<ResponseDto?> SendAsync(RequestDto requestDto, bool jwtBearer=true);
    }
}
