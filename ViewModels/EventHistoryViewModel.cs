namespace EventEaseAssignment.ViewModels
{
    public class EventHistoryViewModel
    {
        public int BookingId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string EventLocation { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string VenueLocation { get; set; } = string.Empty;

        public int Capacity { get; set; }

    }
}
