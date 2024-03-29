﻿using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.ViewModels;
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

        //Data for Loan creation Modal sheet
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.Loans.Include(p => p.LoanType).Include(p => p.DVDCopy).Include(p => p.Member).Include(p => p.DVDCopy.DVDTitle);           
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
               LoanTypeTitle = $"{t.LoanTypeNumber} - {t.LoanTypeName}, {t.LoanDuration} days"
           }), "ID", "LoanTypeTitle");
            return View(await databasecontext.ToListAsync());
        }

        /// <summary>
        /// [FUNCTION 11] Displays a list of all DVD copies on loan currently, along with total loans, ordered by the date out and title.
        /// </summary>
        /// <returns>Renders Relevant View-Page</returns>
        public async Task<IActionResult> CurrentLoan()
        {
            var dvdTitles = _databasecontext.DVDTitles;
            var dvdCopies = _databasecontext.DVDCopies;
            var loans = _databasecontext.Loans;
            var loansPerCopy = from l in loans
                               group l by l.CopyNumber into lg
                               select new
                               {
                                   CopyNumber = lg.Key,
                                   TotalLoans = lg.Count()
                               };

            var query = from t in dvdTitles
                        join d in dvdCopies
                        on t.DVDNumber equals d.DVDNumber
                        join l in loans
                        on d.CopyNumber equals l.CopyNumber
                        join c in loansPerCopy
                        on l.CopyNumber equals c.CopyNumber
                        where l.DateReturned == null
                        orderby l.DateDue, t.DVDTitleName
                        select new DVDCopyLoan()
                        {
                            DVDTitleName = t.DVDTitleName,
                            CopyNumber = d.CopyNumber,
                            MemberName = $"{l.Member.MembershipFirstName} {l.Member.MembershipLastName}",
                            DateOut = l.DateOut,
                            TotalLoans = c.TotalLoans,
                        };

            return View(await query.ToListAsync());
        }

        /// <summary>
        /// [FUNCTION 6] Allow to issue a DVD Copy on loan to a member.
        /// If confirm, Loan is saved
        /// Else, confirmation is asked with a message with loan charges.
        /// </summary>
        /// <param name="memberID">ID of the member who is taking a loan</param>
        /// <param name="dvdCopyID">ID of a DVDCopy record in Database</param>
        /// <param name="loanTypeID">ID of the Loan-Type chooosed by the member, while taking a DVD copy loan</param>
        /// <param name="confirm">Confirm Loan</param>
        /// <returns>Returns the Loan Record or null</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int memberID, int dvdCopyID, int loanTypeID, bool confirm = false)
        {
            var memberRecord = await _databasecontext.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberID);
            var dvdCopyRecord = await _databasecontext.DVDCopies.FirstOrDefaultAsync(c => c.CopyNumber == dvdCopyID);
            var loanTypeRecord = await _databasecontext.LoanTypes.FirstOrDefaultAsync(t => t.LoanTypeNumber == loanTypeID);
            var isdvdCopyLoaned = await (from l in _databasecontext.Loans
                                          where l.CopyNumber == dvdCopyID && l.DateReturned == null
                                          select l).AnyAsync();
            if (isdvdCopyLoaned)
            {
                ViewData["message"] = "Already Loaned!";
                ViewData["error"] = true;
            }
            else
            {
                if (memberRecord != null && dvdCopyRecord != null && loanTypeRecord != null)
                {
                    var memberAge = (DateTime.Now - memberRecord.MemberDOB).TotalDays / 365;
                    var ageRestriction = (await _databasecontext.DVDCategories.FindAsync((await _databasecontext.DVDTitles.FindAsync(dvdCopyRecord.DVDNumber))!.CategoryNumber))!.AgeRestricted;
                    if (bool.Parse(ageRestriction) && memberAge < 18)
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
                                        TotalActiveLoans = lg.Where(l => l.DateReturned == null).Count(),
                                    };
                        var memberLoans = query.Where(l => l.MemberID == memberID).FirstOrDefault();

                        var allowedLoans = (await _databasecontext.MembershipCategories.FindAsync(memberRecord.MembershipCategoryNumber))!.MembershipCategoryTotalLoans;
                        if (memberLoans == null || memberLoans.TotalActiveLoans <= allowedLoans)
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
                                    DateReturned = null,
                                });
                                await _databasecontext.SaveChangesAsync();
                                ViewBag.message = "Successfully Loaned!";
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                var dvdTitle = dvdCopyRecord.DVDTitle;
                                ViewData["message"] = $"The amount to pay for the {dvdTitle.DVDTitleName} copy is Rs.{dvdTitle.StandardCharge * loanDuration} as per the standard charge: Rs.{dvdTitle.StandardCharge}/day for {loanTypeRecord.LoanDuration} days dued at {dateNow.AddDays(loanDuration)}.";
                                ViewData["error"] = false;
                                ViewData["memberID"] = memberID;
                                ViewData["dvdCopyID"] = dvdCopyID;
                                ViewData["loanTypeID"] = loanTypeID;
                            }
                        }
                    }
                }
                else
                {
                    ViewData["message"] = "Something went wrong!";
                    ViewData["error"] = true;
                }
            }
            return View();
        }

        /// <summary>
        /// [FUNCTION 7] Confirm to record the return of a DVD copy. If due date is over the return date, the penalty charge is shown.
        /// </summary>
        /// <param name="dvdCopyID">ID of a DVDCopy record in Database</param>
        /// <returns>Returns the Loan Record or null</returns>
        public async Task<IActionResult> ConfirmReturn(int? dvdCopyID)
        {
            var loan = await ValidateAndGetLoan(dvdCopyID);
            if (loan == null)
            {

                ViewData["message"] = "Either DVD Copy is unavailable or is already returned!";
                ViewData["error"] = true;
            }
            else
            {
                var currentDate = DateTime.Now;
                var dueDate = loan.DateDue;
                ViewData["message"] = "Are you sure the Loaned DVD-Copy is returned?";
                if (currentDate > dueDate)
                {
                    var days = (currentDate - dueDate).TotalDays;
                    var penaltyChargeRate = (await _databasecontext.DVDTitles.FindAsync((await _databasecontext.DVDCopies.FindAsync(loan.CopyNumber))!.DVDNumber))!.PenaltyCharge;
                    var penaltyAmount = Math.Round(days * penaltyChargeRate, 0);
                    ViewData["message"] = $"The Member is penalized with amount: Ns.{penaltyAmount} at the rate of Ns.{penaltyChargeRate}/day for exceeding due date by {Math.Round(days,2)} days.";
                }
                ViewData["error"] = false;
                ViewData["dvdCopyID"] = dvdCopyID;
            }
            return View();
        }

        /// <summary>
        /// [FUNCTION 7] Records the return of a DVD copy
        /// </summary>
        /// <param name="dvdCopyID">ID of a DVDCopy record in Database</param>
        /// <returns>Returns the Loan Record or null</returns>
        public async Task<IActionResult> Return(int? dvdCopyID)
        {
            var loan = await ValidateAndGetLoan(dvdCopyID);
            if (loan == null)
            {
                return NotFound();
            }
            loan.DateReturned = DateTime.Now;
            _databasecontext.Loans.Update(loan);
            await _databasecontext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// [FUNCTION 7] Validates DVDCopy ID and retrieves the Loan corresponding to the DVD copy that is not yet returned.
        /// </summary>
        /// <param name="dvdCopyID">ID of a DVDCopy record in Database</param>
        /// <returns>Returns the Loan Record or null</returns>
        public async Task<Loan?> ValidateAndGetLoan(int? dvdCopyID)
        {
            if (dvdCopyID == null)
            {
                return null;
            }
            var query = from l in _databasecontext.Loans
                        where l.CopyNumber == dvdCopyID && l.DateReturned == null
                        select l;
            return await query.FirstOrDefaultAsync();
        }

        // Confirmation before Loan record deletion
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loanToUpdate = await _databasecontext.Loans.Include(p => p.LoanType).Include(p => p.DVDCopy).Include(p => p.Member).SingleOrDefaultAsync(s => s.LoanNumber == id);
            return View(loanToUpdate);
        }

        // Delete the loan record from the database
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
