using Ecom_Web.Models;
using Ecom_Web.Services.IService;
using Ecom_Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Ecom_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
            
        }
        [HttpGet]
        public IActionResult Register()
        {
            //for role dropdown
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem(text:SD.RoleAdmin,value:SD.RoleAdmin),
                new SelectListItem(text:SD.RoleCustomer,value:SD.RoleCustomer),
            };

            //passing role to view so that we can access it in register view
            ViewBag.RoleList = roleList;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            ResponseDto responseDto = await _authService.RegistrationAsync(registerRequestDto);

            ResponseDto assignRole;

            if (responseDto != null && responseDto.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerRequestDto.Role))
                {
                    registerRequestDto.Role = SD.RoleCustomer;
                }
                // if not empty then we can simple call assignrole
                assignRole = await _authService.RoleAsync(registerRequestDto);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    // then have update success for tempdata and redirect to login
                    TempData["Success"] = "Registration Succesfull";
                    return RedirectToAction(nameof(Login));
                }
            }

            else 
            {
                TempData["Error"] = responseDto.Message;
            }

            // if not register or any issue then redirect to register view with rolw again
            //for role dropdown
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem(text:SD.RoleAdmin,value:SD.RoleAdmin),
                new SelectListItem(text:SD.RoleCustomer,value:SD.RoleCustomer),
            };

            ViewBag.RoleList = roleList;
            return View(registerRequestDto);

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                
                // sign-In user
                await SignIn(loginResponseDto);
                //set the token
                _tokenProvider.setToken(loginResponseDto.Token);
                return RedirectToAction("index", "home");
                

            }

            else
            {
                //ModelState.AddModelError("CustomeError", responseDto.Message);
                //if any error then show notification
                TempData["Error"] = responseDto.Message;
                return View(loginRequestDto);
            }

        }
        public async Task<IActionResult> Logout()
        {
            //for logour use sign-out mehtod
            await HttpContext.SignOutAsync();
            _tokenProvider.clearToken();
            return RedirectToAction("Index","Home");

        }
        [HttpGet]
        public IActionResult AssignRole()
        {
            RegisterRequestDto registerRequestDto = new();
            return View(registerRequestDto);

        }

        //verify user is sign/authenticated or not

        private async Task SignIn(LoginResponseDto loginResponseDto)
        {
            // create jwtsecurity handler

            var tokenHanlder = new JwtSecurityTokenHandler();

            // read token from cookie

            var jwt = tokenHanlder.ReadJwtToken(loginResponseDto.Token);

            // now from this token , we can extract identity 

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            // then add the claim

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            // when we need to work with .net identity then we need to work
            // on one more claim and here we are adding claim type name with email later we can use this while login
            identity.AddClaim(new Claim(ClaimTypes.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            // get tthe role under claim and add it in identity 
            identity.AddClaim(new Claim(ClaimTypes.Role,
               jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            //use SignIn mehtod for microsoft.identity authentication using cookie schem

            var principal = new ClaimsPrincipal(identity);
            //now sign-in using prinicipal
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
