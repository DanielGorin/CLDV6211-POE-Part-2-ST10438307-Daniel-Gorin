// Daniel Gorin
// ST10438307
// CLDV6211 BCAD Group 4

// References:
//             https://www.youtube.com/playlist?list=PL480DYS-b_kevhFsiTpPIB2RzhKPig4iK
//             https://chatgpt.com/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

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
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string containerName = config["AzureStorage:ImageContainer"];
                string imageUrl = await blobService.UploadFileAsync(imageFile, containerName);
                venue.ImageURL = imageUrl;
            }

            ModelState.Remove("ImageURL");

            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }
        //Allows users to EDIT exisitng venues (This section was competed with the assistance of generative AI [chatGPT])
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
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile imageFile, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            if (id != venue.Id)
                return NotFound();

            var originalVenue = await _context.Venue.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            if (originalVenue == null)
                return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(originalVenue.ImageURL))
                    await blobService.DeleteFileAsync(originalVenue.ImageURL, config["AzureStorage:ImageContainer"]);

                string imageUrl = await blobService.UploadFileAsync(imageFile, config["AzureStorage:ImageContainer"]);
                venue.ImageURL = imageUrl;
            }
            else
            {
                venue.ImageURL = originalVenue.ImageURL; // keep existing image
            }

            ModelState.Remove("ImageURL");
            ModelState.Remove("ImageFile");

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
                        return NotFound();
                    else
                        throw;
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

            var venue = await _context.Venue.FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        //Deletes the selected element
        //--------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            var venue = await _context.Venue.FindAsync(id);

            if (venue == null)
            {
                return NotFound();
            }

            // Check if venue is part of a booking
            bool hasBookings = await _context.Booking.AnyAsync(b => b.VenueId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This venue cannot be deleted because it is associated with an existing booking.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            // Delete Blob image if present
            if (!string.IsNullOrEmpty(venue.ImageURL))
            {
                await blobService.DeleteFileAsync(venue.ImageURL, config["AzureStorage:ImageContainer"]);
            }

            // Remove venue
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //Loads the slected venue into a detailed description view (This section was competed with the assistance of generative AI)
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
