using Ecom_ShoppingCartAPI.Model.DTO;

namespace Ecom_ShoppingCartAPI.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
