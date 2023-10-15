using Server_API.Models.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Server_API.Models.Schemas
{
    public class RegisterUserSchema
    {
        [Required(ErrorMessage = "Email is required.")]
        [MinLength(6, ErrorMessage = "Email should be at least 6 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
    }
}
    
