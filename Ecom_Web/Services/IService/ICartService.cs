using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartbyUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartDTO cartDTO);
        Task<ResponseDto?> RemoveCartAsync(int cartDetailId);
        Task<ResponseDto?> ApplyCoupan(CartDTO cartDTO);
        Task<ResponseDto?> EmailCart(CartDTO cartDTO);
    }
}
