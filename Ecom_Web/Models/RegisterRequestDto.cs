using System.ComponentModel.DataAnnotations;

namespace Ecom_Web.Models
{
    public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
