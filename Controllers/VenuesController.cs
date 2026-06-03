/* Code Attribution:
Codes added in this controller were added when the controller was created.
Additional code was added using YouTube videos.
Codes from class were also used to add additional codes.
Codes were also done in class following steps on how to create MVC.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEaseAssignment.Data;
using EventEaseAssignment.Services;

namespace EventEaseAssignment.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseAssignmentContext _context;
        private readonly BlobService _blobService;

        public VenuesController(EventEaseAssignmentContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index(bool? venueAvailable)
        {
            var venues = from v in _context.Venues
                         select v;

            if (venueAvailable.HasValue)
            {
                if (venueAvailable.Value)
                {
                    //Available venues 
                    venues = venues.Where(v => v.Capacity < 50);
                }
                else
                {
                    //Fully booked venues
                    venues = venues.Where(v => v.Capacity >= 50);
                }
            }

            ViewBag.VenueAvailable = venueAvailable;

            return View(await venues.ToListAsync());
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid venue ID.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                TempData["Error"] = "Venue not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venues venues, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    var url = await _blobService.UploadFileAsync(imageFile);
                    venues.VenueImageURL = url;
                }

                var exists = await _context.Venues.AnyAsync(v =>
                    v.VenueName.Trim().ToLower() == venues.VenueName.Trim().ToLower() &&
                    v.VenueLocation.Trim().ToLower() == venues.VenueLocation.Trim().ToLower()
                );

                if (exists)
                {
                    ModelState.AddModelError("", "An event with the same venue name already exists.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(venues);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Venue created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                TempData["Error"] = "Something went wrong while creating the venue.";
            }

            return View(venues);
        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid venue ID.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                TempData["Error"] = "Venue not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venues venues, IFormFile imageFile)
        {
            if (id != venues.VenueId)
                return NotFound();

            try
            {
                if (imageFile != null)
                {
                    var url = await _blobService.UploadFileAsync(imageFile);
                    venues.VenueImageURL = url;
                }

                var exists = await _context.Venues.AnyAsync(v =>
                    v.VenueId != venues.VenueId &&
                    v.VenueName.Trim().ToLower() == venues.VenueName.Trim().ToLower() &&
                    v.VenueLocation.Trim().ToLower() == venues.VenueLocation.Trim().ToLower()
                );

                if (exists)
                {
                    ModelState.AddModelError("", "Another event with the same venue name already exists.");
                }

                if (ModelState.IsValid)
                {
                    _context.Update(venues);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Venue updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                TempData["Error"] = "Something went wrong while updating the venue.";
            }

            return View(venues);
        }

        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Invalid venue ID.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                TempData["Error"] = "Venue not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var venue = await _context.Venues.FindAsync(id);

                if (venue == null)
                {
                    TempData["Error"] = "Venue not found.";
                    return RedirectToAction(nameof(Index));
                }

                var hasBookings = await _context.Events.AnyAsync(e =>
                    e.EventLocation == venue.VenueLocation &&
                    e.Startdate >= DateTime.Now
                );

                if (hasBookings)
                {
                    TempData["Error"] = "Cannot delete this venue because it is used by active or upcoming events.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Venue deleted successfully!";
            }
            catch
            {
                TempData["Error"] = "Error deleting venue.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenuesExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}