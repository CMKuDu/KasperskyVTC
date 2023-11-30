namespace Kaspersky.Infrastructure.Entity.SignUp
{
    public class SignUpModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirPassword { get; set; } = string.Empty;
    }
}
