using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class DVDCopyController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public DVDCopyController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.DVDCopies.Include(p => p.DVDTitle);
            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName");

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dvdcopy)

        {
            try
            {

                dvdcopy.DVDTitle = (await _databasecontext.DVDTitles.FindAsync(dvdcopy.DVDNumber))!;
                Console.WriteLine(dvdcopy);
                _databasecontext.Add(dvdcopy);
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

           
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", dvdcopy.DVDNumber);
            return View(dvdcopy);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var dvdcopy = await _databasecontext.DVDCopies.SingleOrDefaultAsync(s => s.CopyNumber == id);
            return View(dvdcopy);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dvdcopy)
        {
            if (id != dvdcopy.CopyNumber)
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
                _databasecontext.Update(dvdcopy);
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

            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", dvdcopy.DVDNumber);

            return View(dvdcopy);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dvdcopyToUpdate = await _databasecontext.DVDCopies.Include(p => p.DVDTitle).SingleOrDefaultAsync(s => s.CopyNumber == id);
            return View(dvdcopyToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var dvdcopy = await _databasecontext.DVDCopies.FindAsync(id);
            if (dvdcopy == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.DVDCopies.Remove(dvdcopy);
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
