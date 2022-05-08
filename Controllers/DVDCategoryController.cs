using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class DVDCategoryController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public DVDCategoryController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.DVDCategories.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DVDCategory dvdcategory)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(dvdcategory);
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
            return View(dvdcategory);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var dvdcategoryToUpdate = await _databasecontext.DVDCategories.SingleOrDefaultAsync(s => s.CategoryNumber == id);
            return View(dvdcategoryToUpdate);
        }


        //[HttpPost, ActionName("EditPost")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditPost(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var dvdcategoryToUpdate = await _databasecontext.DVDCategories.FirstOrDefaultAsync(s => s.CategoryNumber == id);
        //    if (await TryUpdateModelAsync<DVDCategory>(
        //        dvdcategoryToUpdate,
        //        "",
        //        s => s.CategoryDescription, s => s.AgeRestricted))
        //    {
        //        try
        //        {
        //            await _databasecontext.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException /* ex */)
        //        {
        //            //Log the error (uncomment ex variable name and write a log.)
        //            ModelState.AddModelError("", "Unable to save changes. " +
        //                "Try again, and if the problem persists, " +
        //                "see your system administrator.");
        //        }
        //    }
        //    return View(dvdcategoryToUpdate);
        //}

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dvdcategoryToUpdate = await _databasecontext.DVDCategories.SingleOrDefaultAsync(s => s.CategoryNumber == id);
            return View(dvdcategoryToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var dvdcategory = await _databasecontext.DVDCategories.FindAsync(id);
            if (dvdcategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.DVDCategories.Remove(dvdcategory);
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
