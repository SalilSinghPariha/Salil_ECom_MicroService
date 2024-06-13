using Ecom_Web.Models;

namespace Ecom_Web.Services.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductByCodeAsync(string ProductCode);
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> GetAllProductAsync();
        Task<ResponseDto?> UpdateProductAsync(ProductDTO ProductDto);
        Task<ResponseDto?> DeleteProductAsync(int id);
        Task<ResponseDto?> CreateProductAsync(ProductDTO ProductDto);
    }
}
