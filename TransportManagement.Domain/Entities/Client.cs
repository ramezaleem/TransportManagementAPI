using System.ComponentModel.DataAnnotations;

namespace TransportManagement.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string CompanyName { get; set; }

        [Required, StringLength(30)]
        public string VatCertificateNumber { get; set; }

        [Required, StringLength(30)]
        public string CrNumber { get; set; }

        [Required, StringLength(100)]
        public string DirectorName { get; set; }

        [Required, Phone]
        public string DirectorMobileNumber { get; set; }

        [Required, EmailAddress]
        public string CompanyEmail { get; set; }

        [Required]
        public string VatCertificateCopy { get; set; } // File path

        [Required]
        public string CrCopy { get; set; } // File path

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
