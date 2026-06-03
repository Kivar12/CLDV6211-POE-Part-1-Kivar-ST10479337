/* Code Attribution:
Codes added in this controller were added when the controller was created.
Additional code was added using YouTube videos.
Codes from class were also used to add additional codes.
Codes were also done in class following steps on how to create MVC.
*/

using EventEase.Models;
using EventEaseAssignment.Data;
using EventEaseAssignment.Services;
using EventEaseAssignment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventEaseAssignment.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventEaseAssignmentContext _context;
        private readonly BlobService _blobService;

        public EventsController(EventEaseAssignmentContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        //Index
        public async Task<IActionResult> Index(string searchString,
                                        string eventType,
                                        DateTime? startDate,
                                        DateTime? endDate)
        {
            ViewBag.SearchString = searchString;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            


            var eventsList = _context.Events.AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                eventsList = eventsList.Where(e =>
                    e.EventName.ToLower().Contains(searchString) ||
                    e.EventLocation.ToLower().Contains(searchString));
            }

            //Evnet Type
            if (!string.IsNullOrEmpty(eventType))
            {
                eventType = eventType.ToLower();

                eventsList = eventsList.Where(e =>
                    e.EventName.ToLower().Contains(eventType));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                eventsList = eventsList.Where(e =>
                    e.Startdate.Date <= endDate.Value.Date &&
                    e.Enddate.Date >= startDate.Value.Date);
            }
            else
            {
                if (startDate.HasValue)
                {
                    eventsList = eventsList.Where(e =>
                        e.Startdate.Date <= startDate.Value.Date &&
                        e.Enddate.Date >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    eventsList = eventsList.Where(e =>
                        e.Startdate.Date <= endDate.Value.Date &&
                        e.Enddate.Date >= endDate.Value.Date);
                }
            }

            return View(await eventsList.ToListAsync());
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events @event, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                @event.EventImageURL = await _blobService.UploadFileAsync(imageFile);
            }

            if (@event.Startdate >= @event.Enddate)
            {
                ModelState.AddModelError("", "Start date must be before end date.");
            }

            bool nameExists = await _context.Events.AnyAsync(e =>
                e.EventName == @event.EventName);

            if (nameExists)
            {
                ModelState.AddModelError("EventName", "Event name already exists.");
            }

            bool clash = await _context.Events.AnyAsync(e =>
                e.EventLocation == @event.EventLocation &&
                @event.Startdate < e.Enddate &&
                @event.Enddate > e.Startdate);

            if (clash)
            {
                ModelState.AddModelError("", "Event time clashes at this location.");
            }

            if (!ModelState.IsValid)
                return View(@event);

            _context.Add(@event);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events @event, IFormFile imageFile)
        {
            if (id != @event.EventId)
                return NotFound();

            var existing = await _context.Events.FindAsync(id);

            if (existing == null)
                return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                existing.EventImageURL = await _blobService.UploadFileAsync(imageFile);
            }

            existing.EventName = @event.EventName;
            existing.EventLocation = @event.EventLocation;
            existing.Startdate = @event.Startdate;
            existing.Enddate = @event.Enddate;

            if (existing.Startdate >= existing.Enddate)
            {
                ModelState.AddModelError("", "Start date must be before end date.");
            }

            if (!ModelState.IsValid)
                return View(existing);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
                return NotFound();

            //Block if event has bookings
            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventName == @event.EventName);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete an active or upcoming event.";
                return RedirectToAction(nameof(Index));
            }

            //Block if event is active or upcoming
            bool isActiveOrUpcoming = @event.Enddate > DateTime.Now;

            if (isActiveOrUpcoming)
            {
                TempData["Error"] = "Cannot delete active or upcoming events.";
                return RedirectToAction(nameof(Index));
            }

            //Delete
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}