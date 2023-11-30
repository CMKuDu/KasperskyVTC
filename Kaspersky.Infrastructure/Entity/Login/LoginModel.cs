using System.ComponentModel.DataAnnotations;

namespace Kaspersky.Infrastructure.Entity.Login
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required"), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
