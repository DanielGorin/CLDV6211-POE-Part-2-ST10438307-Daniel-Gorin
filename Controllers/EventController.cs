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
    }
}
