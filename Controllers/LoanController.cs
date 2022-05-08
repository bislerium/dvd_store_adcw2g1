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
            //ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName");
            //ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased");
            //ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber", "MembershipLastName");
            ViewData["DVDCopies"] = new SelectList(_databasecontext.DVDCopies.Select(
           c => new
           {
               ID = c.CopyNumber,
               DVDCopyTitle = $"{c.CopyNumber} - {c.DVDTitle.DVDTitleName}"
           }), "ID", "DVDCopyTitle");
            ViewData["Members"] = new SelectList(_databasecontext.Members.Select(
            m => new
            {
                ID = m.MemberNumber,
                MemberTitle = $"{m.MemberNumber} - {m.MembershipFirstName} {m.MembershipLastName}"
            }), "ID", "MemberTitle");
            ViewData["LoanTypes"] = new SelectList(_databasecontext.LoanTypes.Select(
           t => new
           {
               ID = t.LoanTypeNumber,
               LoanTypeTitle = $"{t.LoanTypeNumber} - {t.LoanDuration}, {t.LoanDuration} days"
           }), "ID", "LoanTypeTitle");
            return View(await databasecontext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("LoanNumber,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)

        //{
        //    try
        //    {
        //        _databasecontext.Add(loan);
        //        await _databasecontext.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));

        //    }
        //    catch (DbUpdateException /* ex */)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        ModelState.AddModelError("", "Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator.");
        //    }

        //    ViewData["LoanTypeNumber"] = new SelectList(_databasecontext.LoanTypes, "LoanTypeNumber", "LoanTypeName", loan.LoanTypeNumber);
        //    ViewData["CopyNumber"] = new SelectList(_databasecontext.DVDCopies, "CopyNumber", "DatePurchased",loan.CopyNumber);
        //    ViewData["MemberNumber"] = new SelectList(_databasecontext.Members, "MemberNumber", "MembershipLastName",loan.MemberNumber);

        //    return View(loan);
        //}

        /// <summary>
        /// Allow to issue a DVD Copy on loan to a member.
        /// If confirm, Loan is saved
        /// Else, confirmation is asked with a message with loan charges.
        /// </summary>
        /// <param name="memberID">ID of the member who is taking a loan</param>
        /// <param name="dvdCopyID">ID of a DVDCopy record in Database</param>
        /// <param name="loanTypeID">ID of the Loan-Type chooosed by the member, while taking a DVD copy loan</param>
        /// <param name="confirm">Confirm Loan</param>
        /// <returns>Returns the Loan Record or null</returns>
        public async Task<IActionResult> Create(int memberID, int dvdCopyID, int loanTypeID, bool confirm = false)
        {
            Console.WriteLine(memberID);
            Console.WriteLine(dvdCopyID);
            Console.WriteLine(loanTypeID);
            var memberRecord = await _databasecontext.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberID);
            var dvdCopyRecord = await _databasecontext.DVDCopies.FirstOrDefaultAsync(c => c.DVDNumber == dvdCopyID);
            var loanTypeRecord = await _databasecontext.LoanTypes.FirstOrDefaultAsync(t => t.LoanTypeNumber == loanTypeID);
            if (memberRecord != null && dvdCopyRecord != null && loanTypeRecord != null)
            {
                var memberAge = (DateTime.Now - memberRecord.MemberDOB).TotalDays / 365;
                Console.WriteLine(dvdCopyRecord);
                Console.WriteLine(dvdCopyRecord.DVDTitle.DVDCategory.AgeRestricted);
                Console.WriteLine(bool.Parse(dvdCopyRecord.DVDTitle.DVDCategory.AgeRestricted));
                if (bool.Parse(dvdCopyRecord.DVDTitle.DVDCategory.AgeRestricted) && memberAge < 18)
                {
                    ViewData["message"] = "Does not meet Age Requirement!";
                    ViewData["error"] = true;
                }
                else
                {
                    var query = from l in _databasecontext.Loans
                                group l by l.MemberNumber into lg
                                select new
                                {
                                    MemberID = lg.Key,
                                    TotalActiveLoans = lg.Where(l => l.DateReturned == DateTime.MinValue).Count(),
                                };
                    var memberLoans = query.Where(l => l.MemberID == memberID).FirstOrDefault();
                    if (memberLoans == null || memberLoans.TotalActiveLoans < memberRecord.MembershipCategory.MembershipCategoryTotalLoans)
                    {
                        var dateNow = DateTime.Now;
                        var loanDuration = Int32.Parse(loanTypeRecord.LoanDuration);
                        if (confirm)
                        {
                            await _databasecontext.Loans.AddAsync(new Loan()
                            {
                                LoanType = loanTypeRecord,
                                DVDCopy = dvdCopyRecord,
                                Member = memberRecord,
                                DateOut = dateNow,
                                DateDue = dateNow.AddDays(loanDuration),
                                DateReturned = DateTime.MinValue,
                            });
                            await _databasecontext.SaveChangesAsync();
                            ViewBag.message = "Successfully Loaned!";
                            return RedirectToRoute(nameof(Index));
                        }
                        else
                        {
                            var dvdTitle = dvdCopyRecord.DVDTitle;
                            ViewData["message"] = $"The amount to pay for the {dvdTitle.DVDTitleName} copy is Rs.{dvdTitle.StandardCharge * loanDuration} as per the standard charge: Rs.{dvdTitle.StandardCharge}/day for {loanTypeRecord.LoanDuration} days dued at {dateNow.AddDays(loanDuration)}.";
                            ViewData["error"] = false;
                            ViewData["memberID"] = memberID;
                            ViewData["dvdCopyID"] = dvdCopyID;
                            ViewData["loanTypeID"] = loanTypeID;
                            ViewData["confirm"] = true;
                        }
                    }
                }
            }
            else
            {
                ViewData["message"] = "Something went wrong!";
                ViewData["error"] = true;
            }
            return View();
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
