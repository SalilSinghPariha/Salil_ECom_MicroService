using Ecom_ShoppingCartAPI.Model.DTO;

namespace Ecom_ShoppingCartAPI.Service
{
    public interface ICoupanService
    {
        Task<CoupanDto> GetCoupan(string coupanCode);
    }
}
