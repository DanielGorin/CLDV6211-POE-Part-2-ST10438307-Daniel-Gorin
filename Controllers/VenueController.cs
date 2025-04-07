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

        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }
        public IActionResult Create()
        {
            return View();
        }
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
    }
}
