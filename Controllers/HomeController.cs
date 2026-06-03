/* Code Attribution:
Codes added in this controller were added when the controller was created.
Additional code was added using YouTube videos.
Codes from class were also used to add additional codes.
Codes were also done in class following steps on how to create MVC.
*/

using EventEaseAssignment.Data;
using EventEaseAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EventEaseAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventEaseAssignmentContext _context;

        public HomeController(EventEaseAssignmentContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalEvents = _context.Events.Count();
            ViewBag.TotalBookings = _context.Bookings.Count();
            ViewBag.TotalVenues = _context.Venues.Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
