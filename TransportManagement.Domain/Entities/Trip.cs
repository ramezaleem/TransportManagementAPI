using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportManagement.Domain.Entities
{
    public class Trip
    {

        // خاصة بالـ Order
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required, StringLength(20)]
        public string Day { get; set; }

        public int DirectionId { get; set; }
        public Direction Direction { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public int TransporterId { get; set; }
        public Transporter Transporter { get; set; }

        [Required, StringLength(100)]
        public string MaterialType { get; set; }

        [Required, StringLength(20)]
        public string MaterialUnit { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaterialQuantity { get; set; }


        // خاصة بالعميل

        [Required, StringLength(100)]
        public string ReceivedBy { get; set; }
        //    public decimal MaterialQuantityOnClientSite { get; set; } 
        public string MaterialArrivedMedia { get; set; } // File paths, JSON or comma separated
        public string ClientMaterialDeliveryReceiptCopy { get; set; } // File path
    }
}
