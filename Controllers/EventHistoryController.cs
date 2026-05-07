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

        public IActionResult Index(string searchString)
        {
            var data = (from e in _context.Events
                        join v in _context.Venues
                        on e.EventLocation.Trim().ToLower() equals v.VenueName.Trim().ToLower()
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

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                data = data.Where(x =>
                    x.BookingId.ToString().Contains(searchString) ||
                    x.EventName.ToLower().Contains(searchString)
                ).ToList();
            }

            return View(data);
        }
    }
}