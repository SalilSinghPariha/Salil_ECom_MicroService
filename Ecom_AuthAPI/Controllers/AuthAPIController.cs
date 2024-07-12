using Ecom.MessageBus;
using Ecom.Services.CoupanAPI.Models.Dto;
using Ecom_AuthAPI.Models.Dto;
using Ecom_AuthAPI.RabbitMQSender;
using Ecom_AuthAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace Ecom_AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService authService;
        //private readonly IMessageBus _messageBus;
        private readonly IRabbitMQAuthSender _rabbitMQAuthSender;
        private readonly IConfiguration _configuration;
        protected ResponseDto responseDto;

        public AuthAPIController(IAuthService authService, IConfiguration configuration,IRabbitMQAuthSender rabbitMQAuthSender)
        {

            this.authService = authService;
            responseDto = new();

            _configuration = configuration;
            //_messageBus = messageBus;
            _rabbitMQAuthSender = rabbitMQAuthSender;

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var errormessage = await authService.Register(registerRequestDto);

            if (!string.IsNullOrEmpty(errormessage))
            {
                responseDto.IsSuccess = false;
                responseDto.Message = errormessage;
                return BadRequest(responseDto);
            }
            //publish message to Queue

             _rabbitMQAuthSender.SendMessage(registerRequestDto.Email,_configuration.GetValue<string>("TopicAndQueueNames:emailRegisterUser"));

            return Ok(responseDto);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await authService.login(loginRequestDto);
            if (loginResponse.UserDto == null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "User Name/Password Incorrect";
                return BadRequest(responseDto);

            }

            responseDto.Result = loginResponse;
            return Ok(responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterRequestDto registerRequestDto)
        {
            var roleResponse = await authService.AssignRole(registerRequestDto.Email,registerRequestDto.Role.ToUpper());
            if (!roleResponse)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Error occorued";
                return BadRequest(responseDto);

            }

            responseDto.Result = roleResponse;
            return Ok(responseDto);
        }
    }
}
