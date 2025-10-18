using System.ComponentModel.DataAnnotations;

namespace TransportManagement.Domain.Entities
{
    public enum TransporterType
    {
        Individual,
        Company
    }

    public class Transporter
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "ID Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ID Number must be exactly 10 digits.")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\+9665[0-9]{8}$", ErrorMessage = "Mobile Number must be in Saudi format, e.g. +9665XXXXXXXX.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Transporter Type is required")]
        public TransporterType TransportType { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Password must contain letters and numbers.")]
        public string Password { get; set; }

        // العلاقات مع المركبات لو عندنا علاقة Entity
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
