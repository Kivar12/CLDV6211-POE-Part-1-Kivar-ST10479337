/* Code Attribution:
Codes added in this controller were added when the controller was created.
Additional code was added using YouTube videos.
Codes from class were also used to add additional codes.
Codes were also done in class following steps on how to create MVC.
*/

using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EventEaseAssignment.Data;
using EventEaseAssignment.ViewModels;

namespace EventEaseAssignment.Controllers
{
    public class EventHistoryController : Controller
    {
        private readonly EventEaseAssignmentContext _context;

        public EventHistoryController(EventEaseAssignmentContext context)
        {
            _context = context;
        }

        public IActionResult Index(
    string searchString,
    string eventType,
    DateTime? startDate,
    DateTime? endDate,
    bool? venueAvailable)
        {
            var data = (from e in _context.Events
                        join v in _context.Venues
                        on e.EventLocation.Trim().ToLower()
                        equals v.VenueName.Trim().ToLower()

                        select new EventHistoryViewModel
                        {
                            BookingId = e.EventId,
                            EventName = e.EventName,
                            EventLocation = e.EventLocation,

                            StartDate = e.Startdate,
                            EndDate = e.Enddate,

                            VenueName = v.VenueName,
                            VenueLocation = v.VenueLocation,
                            Capacity = v.Capacity
                        }).ToList();

            //Existing search
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                data = data.Where(x =>
                    x.BookingId.ToString().Contains(searchString) ||
                    x.EventName.ToLower().Contains(searchString)
                ).ToList();
            }

            //Filter by Event Type
            if (!string.IsNullOrEmpty(eventType))
            {
                eventType = eventType.ToLower();

                data = data.Where(x =>
                    x.EventName.ToLower().Contains(eventType)
                ).ToList();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(x =>
                    x.StartDate.Date >= startDate.Value.Date &&
                    x.EndDate.Date <= endDate.Value.Date
                ).ToList();
            }
            else
            {
                if (startDate.HasValue)
                {
                    data = data.Where(x =>
                        x.StartDate.Date == startDate.Value.Date
                    ).ToList();
                }

                if (endDate.HasValue)
                {
                    data = data.Where(x =>
                        x.EndDate.Date == endDate.Value.Date
                    ).ToList();
                }
            }

            // Venue Availability
            if (venueAvailable.HasValue)
            {
                if (venueAvailable.Value)
                {
                    // Available (less than 50)
                    data = data.Where(x => x.Capacity < 50).ToList();
                }
                else
                {
                    // Fully booked (more than 50)
                    data = data.Where(x => x.Capacity > 50).ToList();
                }
            }

            ViewBag.SearchString = searchString;
            ViewBag.EventType = eventType;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.VenueAvailable = venueAvailable;

            return View(data);
        }
    }
}