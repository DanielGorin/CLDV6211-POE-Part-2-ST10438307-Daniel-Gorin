﻿// Daniel Gorin
// ST10438307
// CLDV6211 BCAD Group 4

// References:
//             https://www.youtube.com/playlist?list=PL480DYS-b_kevhFsiTpPIB2RzhKPig4iK
//             https://chatgpt.com/


using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var events = await _context.Event.Include(e => e.EventType).ToListAsync();
            return View(events);
        }
        //Allows users to CREATE new Event
        //-------------------------------------------------------------------------------------------------------------------------
        //Opens the event create view
        //--------------------------------------------------------------
        public IActionResult Create()
        {
            ViewBag.EventTypes = new SelectList(_context.EventType, "Id", "Category");
            return View();
        }
        //Adds new elements ot the event table
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(Event evnt, IFormFile imageFile, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageURL", "Invalid file type. Only JPG, PNG, GIF, or WEBP images are allowed.");
                    ViewBag.EventTypes = new SelectList(_context.EventType, "Id", "Category", evnt.EventTypeId);
                    return View(evnt);
                }

                string containerName = config["AzureStorage:ImageContainer"];
                string imageUrl = await blobService.UploadFileAsync(imageFile, containerName);
                evnt.ImageURL = imageUrl;
            }

            ModelState.Remove("ImageURL");

            if (ModelState.IsValid)
            {
                _context.Add(evnt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
        }
        //Allows users to EDIT exisitng venues (This section was competed with the assistance of generative AI [chatGPT])
        //-------------------------------------------------------------------------------------------------------------------------
        //Loads the data and Opens the venue edit view
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
            ViewBag.EventTypeId = new SelectList(_context.EventType, "Id", "Category", evnt.EventTypeId);
            return View(evnt);
        }
        //Updates the edited element
        //--------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event evnt, IFormFile imageFile, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {

            

            if (id != evnt.Id)
            {
                return NotFound();
            }

            var originalEvent = await _context.Event.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
           
            if (originalEvent == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageURL", "Invalid file type. Only JPG, PNG, GIF, or WEBP images are allowed.");
                    ViewBag.EventTypes = new SelectList(_context.EventType, "Id", "Category", evnt.EventTypeId);
                    return View(evnt);
                }

                string containerName = config["AzureStorage:ImageContainer"];
                string imageUrl = await blobService.UploadFileAsync(imageFile, containerName);
                evnt.ImageURL = imageUrl;
            }
            else
            {
                evnt.ImageURL = originalEvent.ImageURL;//keeps old img
            }

            ModelState.Remove("ImageFile");

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

            var evnt = await _context.Event.Include(e => e.EventType)
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
        public async Task<IActionResult> DeleteConfirmed(int id, [FromServices] BlobStorageService blobService, [FromServices] IConfiguration config)
        {
            var evnt = await _context.Event.FindAsync(id);

            if (evnt == null)
                return NotFound();

            bool hasBookings = await _context.Booking.AnyAsync(b => b.EventId == id);//check if event is in use

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This event cannot be deleted because it is associated with an existing booking.";
                return RedirectToAction(nameof(Delete), new { id });
            }


            if (!string.IsNullOrEmpty(evnt.ImageURL))//Delete image from Blob if it exists
            {
                await blobService.DeleteFileAsync(evnt.ImageURL, config["AzureStorage:ImageContainer"]);
            }

            _context.Event.Remove(evnt);//Remove from DB
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //Loads the Events into a table for users to VIEW (This section was competed with the assistance of generative AI [chatGPT])
        //-------------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evnt = await _context.Event.Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evnt == null)
            {
                return NotFound();
            }

            return View(evnt);
        }
        //checks that a venue with the selected ID exisits
        //-------------------------------------------------------------------------------------------------------------------------
        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }
    }
}
