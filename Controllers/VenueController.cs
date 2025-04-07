using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Controllers
{
    public class VenueController : Controller
    {
        private readonly EaseDbContext _context;
        public VenueController(EaseDbContext context)
        {
            _context = context;
        }
        //Loads the Venues into a table for users to VIEW
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }
        //Allows users to CREATE new Venues
        //-------------------------------------------------------------------------------------------------------------------------
        //Opens the venue create view
        //--------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }
        //Adds new elements ot the venue table
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        //Allows users to EDIT exisitng venues
        //-------------------------------------------------------------------------------------------------------------------------
        //Loads the data and Opens the venue create view
        //--------------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }
        //Updates the edited element
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
        //Allows users to DELETE exisitng venues
        //-------------------------------------------------------------------------------------------------------------------------
        //Loads the data and Opens the venue delete view
        //--------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }
        //Deletes the selected element
        //--------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        //Loads the Venues into a table for users to VIEW
        //-------------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }
        //checks that a venue with the selected ID exisits
        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.Id == id);
        }
    }
}
