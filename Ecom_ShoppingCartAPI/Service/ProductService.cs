using Ecom_ShoppingCartAPI.Model.DTO;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Ecom_ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

        }
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            // we have defined http client in program.cs file directly with name of Product so
            // once we create client here then it will get url from ther earlier we have
            // created these things through base service under front end project
            var client = _httpClientFactory.CreateClient("Product");
            // call Product API and get the reponse
            var response = await client.GetAsync($"/api/product");
            // get Content
            var apicontent = await response.Content.ReadAsStringAsync();
            // Deserailized object
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apicontent);
            // if response is success then desrialized this reponse back to product dto and send it back
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(resp.Result));

            }

            // if reposne is not success then return new list of productDTO
            return new List<ProductDTO>();


        }
    }
}
