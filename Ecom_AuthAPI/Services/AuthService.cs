using Ecom.Services.AuthAPI.Data;
using Ecom_AuthAPI.Models;
using Ecom_AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace Ecom_AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        //inject identityUser here to perform all the action using dbcontext /applicationuser for
        //usermanager for user and rolemanager for role.

        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext  applicationDbContext,UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            
        }

        public async Task<string> Register(RegisterRequestDto registerRequestDto)
        {
            //assign registerrequestdto to applicaiton user 

            ApplicationUser applicationUser = new()
            {
                UserName= registerRequestDto.Email, 
                Email= registerRequestDto.Email,
                PhoneNumber= registerRequestDto.PhoneNumber,
                Name= registerRequestDto.Name
            };

            try 
            {
                //create user based on user and password anf all hasing
                //and complex things will be taken care by identity itself
                var result = await _userManager.CreateAsync(applicationUser, registerRequestDto.Password);

                if (result.Succeeded)
                {
                    var userToReturn = _context.applicationUsers.FirstOrDefault(u => u.UserName == registerRequestDto.Email);

                    if (userToReturn != null)
                    {
                        UserDto userDto = new()
                        {
                            Id= userToReturn.Id,
                            Name= userToReturn.Name,
                            Email = userToReturn.Email,
                            PhoneNumber = userToReturn.PhoneNumber

                        };

                        return "";
                    }
                }

                return result.Errors.FirstOrDefault().Description;
            }

            catch (Exception ex) 
            {

            }

            return "Error encuntered";
        }

        public  async Task<LoginResponseDto> login(LoginRequestDto loginRequestDto)
        {
            var userInfo = _context.applicationUsers.FirstOrDefault(u=>u.UserName.ToLower()==
            loginRequestDto.UserName.ToLower());

            bool isValid =  await   _userManager.CheckPasswordAsync(userInfo, loginRequestDto.Password);

            if (userInfo==null || isValid ==false)
            {
                return new LoginResponseDto()
                {
                    UserDto=null,
                    Token=""
                };
            }
            // get role through user manager 
            var roles = await _userManager.GetRolesAsync(userInfo);

            // if user found then we need to generate jwt token and pass userinfor with role which we get above
            var token = _jwtTokenGenerator.GenerateToken(userInfo,roles);
            UserDto userDto = new()
            {
                Email=userInfo.Email,
                Id=userInfo.Id,
                PhoneNumber=userInfo.PhoneNumber,
                Name=userInfo.Name, 

            };

            LoginResponseDto loginResponseDto = new()
            {
                UserDto=userDto,
                Token= token
            };

            //return loginresponseDto

            return loginResponseDto;

        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _context.applicationUsers.FirstOrDefault(u => u.Email.ToLower() ==
            email.ToLower());

            if (user != null)
            {
                //we need to use await here since role existsasync in async one so we
                //need to take care for same
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // if role not exist then create role
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                // once role is created then add role to user using _usermanager

               await  _userManager.AddToRoleAsync(user, roleName);

                return true;
            }
            return false;
        }
    }
}
