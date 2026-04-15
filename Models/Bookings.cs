using EventEaseAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Bookings
    {
        [Key]
        public int BookingId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string VenueLocation { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string Email {get; set; }

        public DateTime Date { get; set; }

        public Events? Event { get; set; }

        public Venues? Venues { get; set; }

    }
}
