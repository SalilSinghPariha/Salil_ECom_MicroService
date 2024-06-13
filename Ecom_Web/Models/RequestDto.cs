using Ecom_Web.Utility;
using static Ecom_Web.Utility.SD;

namespace Ecom_Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; }
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
