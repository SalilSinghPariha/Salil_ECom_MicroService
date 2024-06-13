using Ecom_Web.Utility;
using Newtonsoft.Json.Linq;

namespace Ecom_Web.Services.IService
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            
        }
        public void clearToken()
        {
            // delete token from cookie
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? getToken()
        {
            string token = null;

            // the token from request using out as token
            bool? hasToken=_contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
            return hasToken is true? token : null;
        }

        public void setToken(string token)
        {
            // append or set token in cookies
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
        }
    }
}
