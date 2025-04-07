using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Controllers
{
    public class EventController : Controller
    {
        private readonly EaseDbContext _context;
        public EventController(EaseDbContext context)
        {
            _context = context;
        }
        //Loads the Events into a table for users to VIEW
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.ToListAsync();
            return View(events);
        }
        //Allows users to CREATE new Event
        //-------------------------------------------------------------------------------------------------------------------------
        //Opens the event create view
        //--------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }
        //Adds new elements ot the event table
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Event evnt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evnt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
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
            var evnt = await _context.Event.FindAsync(id);
            if (evnt == null)
            {
                return NotFound();
            }
            return View(evnt);
        }
        //Updates the edited element
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event evnt)
        {
            if (id != evnt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evnt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(evnt.Id))
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
            return View(evnt);
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

            var evnt = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evnt == null)
            {
                return NotFound();
            }

            return View(evnt);
        }
        //Deletes the selected element
        //--------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evnt = await _context.Event.FindAsync(id);
            if (evnt != null)
            {
                _context.Event.Remove(evnt);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        //Loads the Events into a table for users to VIEW
        //-------------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evnt = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evnt == null)
            {
                return NotFound();
            }

            return View(evnt);
        }
        //checks that a venue with the selected ID exisits
        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }
    }
}
