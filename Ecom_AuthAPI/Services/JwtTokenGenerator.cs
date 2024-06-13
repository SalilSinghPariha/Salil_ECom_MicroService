using Ecom_AuthAPI.Models;
using Ecom_AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecom_AuthAPI.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // since we added/inject Jwt iptions then using DI we
        // can get jwtoption now here while injecting we need to achive this using Ioption since not added or ijected directly in program.cs
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        { 
            _jwtOptions = jwtOptions.Value;

        }  
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            //use jwtsecuritytoken handler  and get key based on jswtoptios secret key

            var tokenHanlder = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            // this we are having claim and claim types so will configure that also

            var claim = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name,applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Sub,applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email)

            };
            //Add role using claimtype in claim using projection with claimtype as role now we need to pass role
            //while generating token and will get the role woith token as well
            claim.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));

            // security token descriptor that has all properties which we want
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)

            };

            // create token based on token descriptor using token hanlder
            var token = tokenHanlder.CreateToken(tokenDescriptor);

            // return token 

            return tokenHanlder.WriteToken(token);
        }
    }
}
