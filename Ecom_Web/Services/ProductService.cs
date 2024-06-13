using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;

namespace Ecom_Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {

            _baseService = baseService;

        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDTO ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = ProductDto,
                Url = SD.ProductApiBase + "/api/product/AddProduct",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product"
            });
        }

        public async Task<ResponseDto?> GetProductByCodeAsync(string ProductCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/GetByProductCode" + ProductCode
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDTO ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = ProductDto,
                Url = SD.ProductApiBase + "/api/product/UpdateProduct",
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
