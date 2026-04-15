using EventEaseAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Events
    {
        [Key]
        public int EventId { get; set; }

        public  string EventName { get; set; } = string.Empty;

        public  string EventLocation { get; set; } = string.Empty;

        public DateTime Startdate { get; set; }

        public DateTime Enddate { get; set; }

        public string? EventImageURL { get; set; }

        public List<Bookings> Bookings { get; set; } = new List<Bookings>();
    }
}


