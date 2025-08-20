using Microsoft.AspNetCore.Identity;

namespace AuthenticationUserApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;
    }
}
