using AutoMapper;
using Ecom.Service.ProductAPI.Model;
using Ecom.Service.ProductAPI.Model.DTO;

namespace Ecom.Service.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDTO, Product>();
                config.CreateMap<Product, ProductDTO>();
            });

            return mappingConfig;
        }
    }
}
