using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class StudioController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public StudioController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.Studios.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Studio studio)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(studio);
                    await _databasecontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(studio);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var actorToUpdate = await _databasecontext.Studios.SingleOrDefaultAsync(s => s.StudioNumber == id);
            return View(actorToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studioToUpdate = await _databasecontext.Studios.FirstOrDefaultAsync(s => s.StudioNumber == id);
            if (await TryUpdateModelAsync<Studio>(
                studioToUpdate,
                "",
                s => s.StudioName))
            {
                try
                {
                    await _databasecontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studioToUpdate);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studioToUpdate = await _databasecontext.Studios.SingleOrDefaultAsync(s => s.StudioNumber == id);
            return View(studioToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var studio = await _databasecontext.Studios.FindAsync(id);
            if (studio == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Studios.Remove(studio);
                await _databasecontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(DeleteConfirmed), new { id = id, saveChangesError = true });
            }
        }



    }
}
