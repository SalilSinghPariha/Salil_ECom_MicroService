using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface ICoupanService
    {
        Task<ResponseDto?> GetCoupanByCodeAsync(string coupanCode);
        Task<ResponseDto?> GetCoupanByIdAsync(int id);
        Task<ResponseDto?> GetAllCoupanAsync();
        Task<ResponseDto?> UpdateCoupan(CoupanDto coupanDto);
        Task<ResponseDto?> DeleteCoupanAsync(int id);
        Task<ResponseDto?> CreateCoupanAsync(CoupanDto coupanDto);
    }
}
