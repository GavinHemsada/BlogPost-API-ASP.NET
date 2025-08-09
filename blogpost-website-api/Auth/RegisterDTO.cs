using System.ComponentModel.DataAnnotations;

namespace blogpost_website_api.Auth
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|User|Author)$", ErrorMessage = "Role must be Admin, User, or Author")]
        public string Role { get; set; } = "admin";
    }
}
