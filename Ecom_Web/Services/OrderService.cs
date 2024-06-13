using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;

namespace Ecom_Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;

        }
        public async Task<ResponseDto?> CreateOrder(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder"
            });

        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDTO,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}
