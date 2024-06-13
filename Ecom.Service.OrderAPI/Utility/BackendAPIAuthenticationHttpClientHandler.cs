using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Ecom.Service.OrderAPI.Utility
{
    public class BackendAPIAuthenticationHttpClientHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackendAPIAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //we need to call get token async from context accesser and name will be always
            //same as access_token since with this only it have the token
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            // Now we can add the token as header in request
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

            return await  base.SendAsync(request, cancellationToken);
        }

    }
}
