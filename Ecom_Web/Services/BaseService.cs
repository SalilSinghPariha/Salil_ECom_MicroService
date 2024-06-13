using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using static Ecom_Web.Utility.SD;
using System.Net;

namespace Ecom_Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool jwtBearer = true)
        {
            try
            {
                // create httpclient to create request 
                HttpClient httpClient = _httpClientFactory.CreateClient("EcomAPI");

                //Define request and request type for API 
                HttpRequestMessage httpRequestMessage = new();
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    httpRequestMessage.Headers.Add("Accept", "*/*");
                }
                else
                {
                    httpRequestMessage.Headers.Add("Accept", "application/json");
                }
                //Token
                //if jwt bearer then get the value token and add it to message hader for authorization
                if (jwtBearer)
                {
                    var token = _tokenProvider.getToken();
                    httpRequestMessage.Headers.Add("Authorization",$"Bearer {token}" ); //alwys make sure Authorizatin bearer will same otherwise will get an issue
                }
                //For request URI and data

                httpRequestMessage.RequestUri = new Uri(requestDto.Url);
                // if content type has multipart data form image otherwise same json it will take
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {

                    var content = new MultipartFormDataContent();
                    foreach (var prop in requestDto.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDto.Data);
                        if (value is FormFile) // value isform file 
                        {
                            var file = (FormFile)value; // then show the value

                            if (file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()),
                                    prop.Name, file.FileName);
                            }
                        }
                        // if value is not file then add those in content as well otherwise product won't
                        // have all property and it will have only image
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
                    }
                    //after looping then add content to message
                    httpRequestMessage.Content = content;
                }
                else 
                {
                    if (requestDto.Data != null)
                    {
                        httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                    }
                }

                //For response

                HttpResponseMessage? apiResponse = null;

                // for api types , we can switch case
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        httpRequestMessage.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        httpRequestMessage.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        httpRequestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await httpClient.SendAsync(httpRequestMessage);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Forbidden" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "InternalServerError" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        // Deserliazed content and get it to response

                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                        return apiResponseDto;


                }
            }

            catch(Exception ex) 
            {
                var dto = new ResponseDto 
                { 
                    IsSuccess = false, 
                    Message = ex.Message,
                };

                return dto;
            }

        }
    }
}
