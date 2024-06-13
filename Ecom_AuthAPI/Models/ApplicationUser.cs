using Microsoft.AspNetCore.Identity;

namespace Ecom_AuthAPI.Models
{
    //here application user extends identity user to have have our own properties with
    //exisitng identity user properties and when we run migartion then .net core will identify this name
    //and add it under identity 
    public class ApplicationUser : IdentityUser

    {
        public string Name { get; set; }
    }
}
