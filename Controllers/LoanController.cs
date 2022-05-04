using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class LoanController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public LoanController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.Loans.Include(p => p.LoanType).Include(p => p.DVDCopy).Include(p => p.Member);
            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName");
            ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased");
            ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber","MembershipLastName");

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanNumber,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)

        {
            try
            {


                _databasecontext.Add(loan);
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

            ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName", loan.LoanTypeNumber);
            ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased",loan.CopyNumber);
            ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber", "MembershipLastName",loan.MemberNumber);

            return View(loan);
        }


        public async Task<IActionResult> EditPost(int id)
        {
            var loan = await _databasecontext.Loans.SingleOrDefaultAsync(s => s.LoanNumber == id);
            ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName", loan.LoanTypeNumber);
            ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased", loan.CopyNumber);
            ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber", "MembershipLastName", loan.MemberNumber);
            return View(loan);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("LoanNumber,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        {
            if (id != loan.LoanNumber)
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
                _databasecontext.Update(loan);
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

            ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName", loan.LoanTypeNumber);
            ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased", loan.CopyNumber);
            ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber", "MembershipLastName", loan.MemberNumber);

            return View(loan);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loanToUpdate = await _databasecontext.Loans.Include(p => p.LoanType).Include(p => p.DVDCopy).Include(p => p.Member).SingleOrDefaultAsync(s => s.LoanNumber == id);
            return View(loanToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var loan = await _databasecontext.Loans.FindAsync(id);
            if (loan == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Loans.Remove(loan);
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
