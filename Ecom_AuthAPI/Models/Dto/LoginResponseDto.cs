namespace Ecom_AuthAPI.Models.Dto
{
    public class LoginResponseDto
    {

        public UserDto UserDto { get; set; }
        public string Token { get; set; }
    }
}
