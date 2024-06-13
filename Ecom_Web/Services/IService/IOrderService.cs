using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDTO cartDTO);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);


    }
}
