using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class DVDTitleController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public DVDTitleController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.DVDTitles.Include(p => p.Producer).Include(p => p.DVDCategory).Include(p => p.Studio);
            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName");
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription");
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DVDNumber,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dvdtitle)

        {
            try
            {
              
                    
                    _databasecontext.Add(dvdtitle);
                    await _databasecontext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName", dvdtitle.ProducerNumber);
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription", dvdtitle.CategoryNumber);
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName", dvdtitle.StudioNumber);

            return View(dvdtitle);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var dvdtitleToUpdate = await _databasecontext.DVDTitles.SingleOrDefaultAsync(s => s.DVDNumber == id);
            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName", dvdtitleToUpdate.ProducerNumber);
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription", dvdtitleToUpdate.CategoryNumber);
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName", dvdtitleToUpdate.StudioNumber);
            return View(dvdtitleToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("DVDNumber,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dvdtitle)
        {
            if (id != dvdtitle.DVDNumber)
            {
                return NotFound();
            }
            //var dvdtitleToUpdate = await _databasecontext.DVDTitles.FirstOrDefaultAsync(s => s.DVDNumber == id);
            //if (await TryUpdateModelAsync<DVDTitle>(
            //    dvdtitleToUpdate,
            //    "",
            //    s => s.ProducerNumber, s => s.CategoryNumber, s => s.StudioNumber, s => s.DateReleased, s => s.StandardCharge, s => s.PenaltyCharge))
            
                try
                {
                    _databasecontext.Update(dvdtitle);
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
            
            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName", dvdtitle.ProducerNumber);
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription", dvdtitle.CategoryNumber);
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName", dvdtitle.StudioNumber);
            
            return View(dvdtitle);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dvdtitleToUpdate = await _databasecontext.DVDTitles.Include(p => p.Producer).Include(p => p.DVDCategory).Include(p => p.Studio).SingleOrDefaultAsync(s => s.DVDNumber == id);
            return View(dvdtitleToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var dvdtitle = await _databasecontext.DVDTitles.FindAsync(id);
            if (dvdtitle == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.DVDTitles.Remove(dvdtitle);
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
