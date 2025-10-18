using System.ComponentModel.DataAnnotations;

namespace TransportManagement.Domain.Entities
{
    public class Direction
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string DirectionName { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
