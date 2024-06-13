using Ecom_ShoppingCartAPI.Model.DTO;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Ecom_ShoppingCartAPI.Service
{
    public class CoupanService : ICoupanService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CoupanService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

        }

        public async Task<CoupanDto> GetCoupan(string coupanCode)
        {
            var client = _httpClientFactory.CreateClient("Coupan");
            var response = await client.GetAsync($"/api/coupan/GetByCoupanCode/{coupanCode}");
            var apicontent= await response.Content.ReadAsStringAsync(); 
            var resp= JsonConvert.DeserializeObject<ResponseDto>(apicontent);

            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CoupanDto>(Convert.ToString(resp.Result));
            }

            return new CoupanDto();

        }
    }
}
