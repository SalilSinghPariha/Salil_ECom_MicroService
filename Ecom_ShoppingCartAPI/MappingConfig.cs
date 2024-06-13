using Ecom_ShoppingCartAPI.Model;
using Ecom_ShoppingCartAPI.Model.DTO;
using AutoMapper;

namespace Ecom_ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader,CartHeaderDTO>().ReverseMap(); //reverse map for vice versa
                config.CreateMap<CartDetails, CartDetailDTO>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
