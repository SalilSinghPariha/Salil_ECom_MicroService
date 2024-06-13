using AutoMapper;
using Ecom.Service.OrderAPI.Model;
using Ecom.Service.OrderAPI.Model.DTO;

namespace Ecom.Service.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailDTO, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.ProductDTO.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.ProductDTO.Price));

                config.CreateMap<OrderDetailsDto, CartDetailDTO>();

                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();

            });
            return mappingConfig;
        }
    }
}
