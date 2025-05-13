// Daniel Gorin
// ST10438307
// CLDV6211 BCAD Group 4

// References:
//             https://www.youtube.com/playlist?list=PL480DYS-b_kevhFsiTpPIB2RzhKPig4iK
//             https://chatgpt.com/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Controllers
{
    public class BookingController : Controller
    {
        private readonly EaseDbContext _context;
        public BookingController(EaseDbContext context)
        {
            _context = context;
        }
        //Loads the Bookings into a table for users to VIEW
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index(string searchString)
        {
            var bookingsQuery = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Try to parse as Booking ID
                if (int.TryParse(searchString, out int bookingId))
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Id == bookingId);
                }
                else
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Event.Name.Contains(searchString));
                }
            }

            var bookings = await bookingsQuery.ToListAsync();
            return View(bookings);
        }
        //Allows users to CREATE new Bookings
        //-------------------------------------------------------------------------------------------------------------------------
        //Opens the booking create view
        //--------------------------------------------------------------
        public async Task<IActionResult> Create()
        {
            var bookedEventIds = await _context.Booking
                .Select(b => b.EventId)
                .ToListAsync();

            var availableEvents = await _context.Event
                .Where(e => !bookedEventIds.Contains(e.Id))
                .ToListAsync();

            var venues = await _context.Venue.ToListAsync();

            ViewBag.AvailableEvents = availableEvents;
            ViewBag.AllVenues = venues;

            return View();
        }
        //Adds new elements ot the booking table
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            //ModelState.Remove("Venue");//overides validation
            // ModelState.Remove("Event");
            var eventDate = await _context.Event
                .Where(e => e.Id == booking.EventId)
                .Select(e => e.Date)
                .FirstOrDefaultAsync();

            bool isVenueBooked = await _context.Booking
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueId == booking.VenueId && b.Event.Date == eventDate);

            if (isVenueBooked)
            {
                ModelState.AddModelError("VenueId", "This venue is already booked for the selected event date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var bookedEventIds = await _context.Booking
                .Select(b => b.EventId)
                .ToListAsync();

            var availableEvents = await _context.Event
                .Where(e => !bookedEventIds.Contains(e.Id))
                .ToListAsync();

            var venues = await _context.Venue.ToListAsync();

            ViewBag.AvailableEvents = availableEvents;
            ViewBag.AllVenues = venues;

            return View(booking); // Re-show form with error messages
        }
        //Allows users to EDIT exisitng bookings (This section was competed with the assistance of generative AI [chatGPT])
        //-------------------------------------------------------------------------------------------------------------------------
        //Loads the data and Opens the booking edit view
        //--------------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookedVenueDates = await _context.Booking
                .Include(b => b.Event)
                .Where(b => b.Id != booking.Id) // exclude current booking
                .Select(b => new {
                    VenueId = b.VenueId,
                    Date = b.Event.Date.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            var allEvents = await _context.Event.ToListAsync();
            var allVenues = await _context.Venue.ToListAsync();

            ViewBag.AllEvents = allEvents;
            ViewBag.AllVenues = allVenues;
            ViewBag.BookedVenueDates = bookedVenueDates;

            return View(booking);
        }
        //Updates the edited element
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.Id)
                return NotFound();

            var eventDate = await _context.Event
                .Where(e => e.Id == booking.EventId)
                .Select(e => e.Date)
                .FirstOrDefaultAsync();

            bool isVenueBooked = await _context.Booking
                .Include(b => b.Event)
                .AnyAsync(b =>
                    b.Id != booking.Id &&
                    b.VenueId == booking.VenueId &&
                    b.Event.Date == eventDate
                );

            if (isVenueBooked)
            {
                ModelState.AddModelError("VenueId", "This venue is already booked on the selected event date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            var allEvents = await _context.Event.ToListAsync();
            var allVenues = await _context.Venue.ToListAsync();
            var bookedVenueDates = await _context.Booking
                .Include(b => b.Event)
                .Where(b => b.Id != booking.Id)
                .Select(b => new {
                    VenueId = b.VenueId,
                    Date = b.Event.Date.ToString("yyyy-MM-dd")
                }).ToListAsync();

            ViewBag.AllEvents = allEvents;
            ViewBag.AllVenues = allVenues;
            ViewBag.BookedVenueDates = bookedVenueDates;

            return View(booking);
        }
        //Allows users to DELETE exisitng bookings
        //-------------------------------------------------------------------------------------------------------------------------
        //Loads the data and Opens the booking delete view
        //--------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        //Deletes the selected element
        //--------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        //Loads the slected booking into a detailed description view
        //-------------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        //checks that a booking with the selected ID exisits
        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
