using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class LoanTypeController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public LoanTypeController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.LoanTypes.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanType loantype)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(loantype);
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
            return View(loantype);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var loantypeToUpdate = await _databasecontext.LoanTypes.SingleOrDefaultAsync(s => s.LoanTypeNumber == id);
            return View(loantypeToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var loantypeToUpdate = await _databasecontext.LoanTypes.FirstOrDefaultAsync(s => s.LoanTypeNumber == id);
            if (await TryUpdateModelAsync<LoanType>(
                loantypeToUpdate,
                "",
                s => s.LoanTypeName, s => s.LoanDuration))
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
            return View(loantypeToUpdate);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loantypeToUpdate = await _databasecontext.LoanTypes.SingleOrDefaultAsync(s => s.LoanTypeNumber == id);
            return View(loantypeToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var loantype = await _databasecontext.LoanTypes.FindAsync(id);
            if (loantype == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.LoanTypes.Remove(loantype);
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
