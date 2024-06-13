using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;

namespace Ecom_Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {

            _baseService = baseService;

        }

        public async Task<ResponseDto?> ApplyCoupan(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.CartAPIBase + "/api/cart/ApplyCoupan"
            });
        }

        public async Task<ResponseDto?> GetCartbyUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CartAPIBase + "/api/cart/GetCart/" + userId
            });
        }

        public async Task<ResponseDto?> RemoveCartAsync(int cartDetailId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailId,
                Url = SD.CartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.CartAPIBase + "/api/cart/CartUpsert"
            });
        }

        public async Task<ResponseDto?> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.CartAPIBase + "/api/cart/EmailCartRequest"
            });
        }
    }
}
