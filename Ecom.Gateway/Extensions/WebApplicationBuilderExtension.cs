using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ecom.Gateway.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        //extension mehtod alswys static and we can call webapplicationBuilder extension class with this keyword

        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            // here will paster our code

            var Secret = builder.Configuration.GetValue<string>("JwtSetting:Secret");
            var Issuer = builder.Configuration.GetValue<string>("JwtSetting:Issuer");
            var Audience = builder.Configuration.GetValue<string>("JwtSetting:Audience");

            var key = Encoding.ASCII.GetBytes(Secret);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {

                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                };
            });

            return builder;

        }
    }
}
