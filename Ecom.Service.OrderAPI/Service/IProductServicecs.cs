using Ecom.Service.OrderAPI.Model.DTO;

namespace Ecom.Service.OrderAPI.Service
{
    public interface IProductServicecs
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
