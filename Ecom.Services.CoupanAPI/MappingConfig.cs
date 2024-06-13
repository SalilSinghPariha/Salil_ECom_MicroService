using Ecom.Services.CoupanAPI.Models;
using Ecom.Services.CoupanAPI.Models.Dto;
using AutoMapper;

namespace Ecom.Services.CoupanAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CoupanDto,Coupan>();
                config.CreateMap<Coupan, CoupanDto>();
            });

            return mappingConfig;
        }
    }
}
