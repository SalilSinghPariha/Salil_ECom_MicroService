using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;

namespace Ecom_Web.Services
{
    public class CoupanService : ICoupanService
    {
        private readonly IBaseService _baseService;
        public CoupanService(IBaseService baseService)
        {

            _baseService = baseService;

        }
        public async Task<ResponseDto?> CreateCoupanAsync(CoupanDto coupanDto)
        {
            return await _baseService.SendAsync(new RequestDto() 
            { 
                ApiType=SD.ApiType.POST,
                Data=coupanDto,
                Url=SD.CoupanApiBase+ "/api/coupan/AddCoupan"
            });
        }

        public async Task<ResponseDto?> DeleteCoupanAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto() 
            { 
                ApiType= SD.ApiType.DELETE,
                Url= SD.CoupanApiBase+ "/api/coupan/" + id
            });
        }

        public async Task<ResponseDto?> GetAllCoupanAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType=SD.ApiType.GET,
                Url=SD.CoupanApiBase+"/api/coupan"
            });

        }

        public async Task<ResponseDto?> GetCoupanByCodeAsync(string coupanCode)
        {
            return await _baseService.SendAsync(new RequestDto() 
            {
                ApiType= SD.ApiType.GET,
                Url= SD.CoupanApiBase+ "/api/coupan/GetByCoupanCode" + coupanCode
            });
        }

        public async Task<ResponseDto?> GetCoupanByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType= SD.ApiType.GET,
                Url=SD.CoupanApiBase+"/api/coupan/"+id
            });
        }

        public async Task<ResponseDto?> UpdateCoupan(CoupanDto coupanDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            { 
                ApiType=SD.ApiType.PUT,
                Data=coupanDto,
                Url=SD.CoupanApiBase+ "/api/coupan/UpdateCoupan"
            });
        }
    }
}
