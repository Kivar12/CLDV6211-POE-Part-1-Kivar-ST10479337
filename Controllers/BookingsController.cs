/* Code Attribution:
Codes added in this controller were added when the controller was created.
Additional code was added using YouTube videos.
Codes from class were also used to add additional codes.
Codes were also done in class following steps on how to create MVC.
*/
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEaseAssignment.Data;

namespace EventEaseAssignment.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseAssignmentContext _context;

        public BookingsController(EventEaseAssignmentContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchString,
                                        DateTime? startDate,
                                        DateTime? endDate,
                                        string eventType)
        {
            ViewBag.SearchString = searchString;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.EventType = eventType;

            var bookings = from b in _context.Bookings
                           select b;

            //Search
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.EventName.Contains(searchString) ||
                    b.VenueLocation.Contains(searchString) ||
                    b.CustomerName.Contains(searchString));
            }

            //Event Type
            if (!string.IsNullOrEmpty(eventType))
            {
                bookings = bookings.Where(b =>
                    b.EventName.Contains(eventType));
            }

            //Start Date
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Date >= startDate.Value);
            }

            //End Date
            if (endDate.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Date <= endDate.Value);
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventName,VenueLocation,CustomerName,Email,Date")] Bookings bookings)
        {
            try
            {
                bool nameExists = await _context.Bookings
                    .AnyAsync(b => b.EventName == bookings.EventName);

                if (nameExists)
                {
                    ModelState.AddModelError("EventName", "Event name already exists.");
                }

                bool venueClash = await _context.Bookings.AnyAsync(b =>
                    b.VenueLocation == bookings.VenueLocation &&
                    b.Date == bookings.Date
                );

                if (venueClash)
                {
                    ModelState.AddModelError("", "This venue is already booked at this date and time.");
                }

                if (!ModelState.IsValid)
                {
                    return View(bookings);
                }

                _context.Add(bookings);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Something went wrong while creating the booking.";
                return View(bookings);
            }
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.FindAsync(id);

            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventName,VenueLocation,CustomerName,Email,Date")] Bookings bookings)
        {
            if (id != bookings.BookingId)
            {
                return NotFound();
            }

            try
            {
                bool nameExists = await _context.Bookings
                    .AnyAsync(b => b.EventName == bookings.EventName && b.BookingId != bookings.BookingId);

                if (nameExists)
                {
                    ModelState.AddModelError("EventName", "Event name already exists.");
                }

                bool venueClash = await _context.Bookings.AnyAsync(b =>
                    b.BookingId != bookings.BookingId &&
                    b.VenueLocation == bookings.VenueLocation &&
                    b.Date == bookings.Date
                );

                if (venueClash)
                {
                    ModelState.AddModelError("", "This venue is already booked at this date and time.");
                }

                if (!ModelState.IsValid)
                {
                    return View(bookings);
                }

                _context.Update(bookings);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Something went wrong while updating the booking.";
                return View(bookings);
            }
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (booking.Date >= DateTime.Now)
                {
                    TempData["Error"] = "Cannot delete an active or upcoming event.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error occurred while deleting booking.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool BookingsExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}