using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape4.Models.DTO
{
    public class RegisterRequest
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(100)]
        public string MotDePasse { get; set; } = string.Empty;

        [Required, MaxLength(45)]
        public string Pseudo { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasse { get; set; } = string.Empty;
    }
}