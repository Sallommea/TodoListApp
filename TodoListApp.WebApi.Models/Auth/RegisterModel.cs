using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Auth;
public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])(?=.{8,100}).{8,100}$",
        ErrorMessage = "Password must be 8-100 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name must be between {2} and {1} characters.", MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s'-]+$", ErrorMessage = "First name can only contain letters, spaces, hyphens, and apostrophes.")]
    public string Firstname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name must be between {2} and {1} characters.", MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s'-]+$", ErrorMessage = "Last name can only contain letters, spaces, hyphens, and apostrophes.")]
    public string Lastname { get; set; } = string.Empty;
}
