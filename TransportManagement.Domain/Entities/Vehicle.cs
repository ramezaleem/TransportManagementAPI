using System.ComponentModel.DataAnnotations;

namespace TransportManagement.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vehicle Type is required")]
        [StringLength(50)]
        public string VehicleType { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50)]
        public string Model { get; set; }

        [Required(ErrorMessage = "Plate No is required")]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "Plate No must be between 1 and 4 digits.")]
        public string PlateNo { get; set; }

        // خاصية محسوبة تعبأ الأصفار على اليسار ليصبح الرقم بأربع خانات دائمًا
        public string PlateNoPadded
        {
            get
            {
                if (string.IsNullOrEmpty(PlateNo))
                    return "0000";
                return PlateNo.PadLeft(4, '0');
            }
        }

        [Required(ErrorMessage = "Driver Name is required")]
        [StringLength(100)]
        public string DriverName { get; set; }

        [Required(ErrorMessage = "Driver ID Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Driver ID must be exactly 10 digits.")]
        public string DriverIdNumber { get; set; }

        [Required(ErrorMessage = "Driver Mobile Number is required")]
        [RegularExpression(@"^\+\d{1,3}\s\d{6,15}$", ErrorMessage = "Driver Mobile Number must be in international format, e.g. +966 543112826.")]
        public string DriverMobileNumber { get; set; }

        [Required(ErrorMessage = "Vehicle License Copy is required")]
        public string VehicleLicenseCopy { get; set; } // File path or URL

        [Required(ErrorMessage = "Driver License Copy is required")]
        public string DriverLicenseCopy { get; set; } // File path or URL

        [Required]
        public int TransporterId { get; set; }
        public Transporter Transporter { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
