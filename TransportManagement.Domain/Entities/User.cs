using System.ComponentModel.DataAnnotations;

namespace TransportManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }

        public string? EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;

        [Required, Phone, StringLength(20)]
        public string MobileNumber { get; set; }

        public bool IsMobileConfirmed { get; set; } = false;
        public string? MobileOTP { get; set; }
        public DateTime? MobileOTPExpiry { get; set; }

        [Required, StringLength(20)]
        public string TransporterType { get; set; }

        [Required, StringLength(10, MinimumLength = 10)]
        public string NationalId { get; set; }

        // ---- الجديد ----
        public string? EmailLoginCode { get; set; }
        public DateTime? EmailLoginCodeExpiry { get; set; }
    }
}
