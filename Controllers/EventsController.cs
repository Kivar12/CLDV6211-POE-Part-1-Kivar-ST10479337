using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEaseAssignment.Data;
using EventEaseAssignment.Services;

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

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // ===================== CREATE =====================
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

        // ===================== EDIT =====================
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

        // ===================== DELETE =====================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // ===================== DELETE CONFIRMED (FIXED LOGIC) =====================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
                return NotFound();

            // 1. BLOCK IF EVENT HAS BOOKINGS
            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventName == @event.EventName);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete an active or upcoming event.";
                return RedirectToAction(nameof(Index));
            }

            // 2. BLOCK IF EVENT IS ACTIVE OR IN FUTURE
            bool isActiveOrUpcoming = @event.Enddate > DateTime.Now;

            if (isActiveOrUpcoming)
            {
                TempData["Error"] = "Cannot delete active or upcoming events.";
                return RedirectToAction(nameof(Index));
            }

            // 3. DELETE
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}