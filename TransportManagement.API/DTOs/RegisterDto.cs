using System.ComponentModel.DataAnnotations;

namespace TransportManagement.API.DTOs
{
    public class RegisterDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20), Phone]
        public string MobileNumber { get; set; }

        [Required, StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "NationalId must be exactly 10 digits.")]
        public string NationalId { get; set; }

        [Required, StringLength(20)]
        public string TransporterType { get; set; } // "Individual" or "Company"

        [Required, StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters and contain both letters and numbers.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
