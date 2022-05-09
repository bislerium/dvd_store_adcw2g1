using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.ViewModels;
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
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName");
            return View(await databasecontext.ToListAsync());
        }

        /// <summary>
        /// [FUNCTION 5] Allow the user to enter or select a Copy by CopyNumber and find the details of the last loan for that copy(whether it is still on loan or has been returned), ie the Name of the Member who borrowed it, the date out, due and back(if any) and the title of the DVD.
        /// </summary>
        /// <param name="id">DVD Copy ID</param>
        /// <returns>Renders Relevant Page</returns>
        public async Task<IActionResult> Details(int? id)
        {
            var databasecontext = from l in _databasecontext.Loans
                                  join m in _databasecontext.Members
                                  on l.MemberNumber equals m.MemberNumber
                                  join c in _databasecontext.DVDCopies
                                  on l.CopyNumber equals c.CopyNumber
                                  join t in _databasecontext.DVDTitles
                                  on c.DVDNumber equals t.DVDNumber
                                  where l.CopyNumber == id
                                  group l by c.CopyNumber into lg
                                  select new RecentDVDCopyLoan()
                                  {
                                      Loan = lg.OrderBy(l => l.DateOut).Last(),
                                      Member = (from m in _databasecontext.Members
                                                where m.MemberNumber == lg.OrderBy(l => l.DateOut).Last().MemberNumber
                                                select m).First(),
                                      DVDTitle = (from t in _databasecontext.DVDTitles
                                                  join c in _databasecontext.DVDCopies
                                                  on t.DVDNumber equals c.DVDNumber
                                                  where c.CopyNumber == lg.OrderBy(l => l.DateOut)!.Last().CopyNumber
                                                  select t).First(),
                                  };

            return View(await databasecontext.FirstOrDefaultAsync());
        }

        // Create a DVDCopy record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dvdcopy)

        {
            try
            {
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

        /// <summary>
        /// [FUNCTION 10] Displays a list of all DVD Copies which are more than a year old and which are not currently on loan b.
        /// </summary>
        /// <returns>Renders Relevant View-Page</returns>
        public async Task<IActionResult> YearOldDVDCopies()
        {
            var dvdCopies = _databasecontext.DVDCopies;
            var loans = _databasecontext.Loans;
            var query = from l in loans
                        join d in dvdCopies
                        on l.CopyNumber equals d.CopyNumber
                        where d.DatePurchased.AddDays(365) < DateTime.Now
                        group l by l.CopyNumber into lg
                        select new OldDVDCopy
                        {
                            DVDCopy = (from d in _databasecontext.DVDCopies
                                       where d.CopyNumber == lg.Key
                                       select d).First(),
                            DVDTitle = (from dt in _databasecontext.DVDTitles
                                        join d in dvdCopies
                                        on dt.DVDNumber equals d.DVDNumber
                                        where d.CopyNumber == lg.Key
                                        select dt).First(),
                            IsLoaned = lg.Any(l => l.DateReturned == null),
                        };

            return View(await query.Where(o => !o.IsLoaned).ToListAsync());
        }

        // Filled data form for Editing Post
        public async Task<IActionResult> EditPost(int id)
        {
            var dvdcopy = await _databasecontext.DVDCopies.SingleOrDefaultAsync(s => s.CopyNumber == id);
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", dvdcopy!.DVDNumber);
            return View(dvdcopy);
        }

        // Edit DVDCopy record
        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dvdcopy)
        {
            if (id != dvdcopy.CopyNumber)
            {
                return NotFound();
            }
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

        // Confirm the deletion of DVDCopy record
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dvdcopyToUpdate = await _databasecontext.DVDCopies.Include(p => p.DVDTitle).SingleOrDefaultAsync(s => s.CopyNumber == id);
            return View(dvdcopyToUpdate);
        }

        // Delete the DVDCopy record
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
