using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venues
    {
        [Key]
        public int VenueId { get; set; }

        public string VenueLocation { get; set; } = string.Empty;

        public string VenueName { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string? VenueImageURL { get; set; }

        public List<Bookings> Bookings { get; set; } = new List<Bookings>();
    }
}