using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class MemberController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public MemberController(DatabaseContext context)
        {
            _databasecontext = context;
        }

        // GET all Members or Member(s) by the lastname
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription");
            if (!String.IsNullOrEmpty(searchString))
            {
                var filtered = _databasecontext.Members.Include(p => p.MembershipCategory).Where(m => m.MembershipLastName.Contains(searchString));
                return View(await filtered.ToListAsync());
            }
            var members = _databasecontext.Members.Include(p => p.MembershipCategory);
            return View(await members.ToListAsync());
        }

        // GET detail of a member by a ID
        public async Task<IActionResult> Details(int? id)
        {
            var databasecontext = from s in _databasecontext.Loans.Include(p => p.DVDCopy).Include(p => p.Member).Include(p => p.DVDCopy.DVDTitle) select s;

            var today = DateTime.Today.AddDays(-31);

            databasecontext = databasecontext.Where(s => s.DateOut >= today);

            if (!id.Equals(null))
            {
                databasecontext = databasecontext.Where(s => s.Member.MemberNumber.Equals(id));

            }

            return View(await databasecontext.ToListAsync());
        }

        /// <summary>
        /// [FUNCTION 8] Displays an alphabetic list of all members (including members with no current loans) with all their details(with membership category decoded) and the total number of DVDs they currently have on loan.This report should highlight with the message “Too many DVDs” any member who has more DVDs on loan than they are allowed from their MembershipCategor
        /// </summary>
        /// <returns>Renders Relevant View-Page</returns>
        public async Task<IActionResult> MemberLoanDetails()
        {
            var loans = _databasecontext.Loans;
            var Members = _databasecontext.Members;
            var query = from m in Members
                        join l in loans.DefaultIfEmpty()
                        on m.MemberNumber equals l.MemberNumber
                        group l by m.MemberNumber into lg
                        select new MembersActiveLoans
                        {
                            Member = (from m in _databasecontext.Members
                                      where m.MemberNumber == lg.Key
                                      select m).First(),
                            MemberCategory = (from mc in _databasecontext.MembershipCategories
                                              join ms in _databasecontext.Members
                                              on mc.MembershipCategoryNumber equals ms.MembershipCategoryNumber
                                              where ms.MemberNumber == lg.Key
                                              select mc).First(),
                            TotalActiveLoans = lg.Where(l => l.DateReturned == null).Count(),
                        };

            return View(await query.ToListAsync());
        }

        /// <summary>
        /// [FUNCTION 12] Displays a list of all Members who have not borrowed any DVD in the last 31 days, ignoring any Member who has never borrowed a DVD.
        /// </summary>
        /// <returns>Renders Relevant View-Page</returns>
        public async Task<IActionResult> InactiveMember()
        {
            var loans = _databasecontext.Loans;
            var query = from l in loans
                        group l by l.MemberNumber into lg
                        select new InActiveLoanMember()
                        {
                            Member = (from m in _databasecontext.Members
                                      where m.MemberNumber == lg.Key
                                      select m).First(),
                            LastDateOut = lg.Max(a => a.DateOut),
                            LastLoanedDVDTitleName = (from t in _databasecontext.DVDTitles
                                                      join c in _databasecontext.DVDCopies
                                                      on t.DVDNumber equals c.DVDNumber
                                                      join l in _databasecontext.Loans
                                                      on c.CopyNumber equals l.CopyNumber
                                                      where l.MemberNumber == lg.Key && l.DateOut == lg.Max(a => a.DateOut)
                                                      select t.DVDTitleName).First(),
                            DaysSinceLastLoaned = Math.Round((DateTime.Now - lg.Max(l => l.DateOut)).TotalDays, 2),
                        };

            return View(await query.Where(a => (DateTime.Now > a.LastDateOut.AddDays(31))).ToListAsync());
        }

        // Create a Member recoord in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberNumber,MembershipCategoryNumber,MembershipLastName,MembershipFirstName,MembershipAddress,MemberDOB")] Member member)

        {
            try
            {


                _databasecontext.Add(member);
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

            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription", member.MembershipCategoryNumber);

            return View(member);
        }

        // Filled form-data in Modalsheet for Members record
        public async Task<IActionResult> EditPost(int id)
        {
            var member = await _databasecontext.Members.SingleOrDefaultAsync(s => s.MemberNumber == id);
            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription", member.MembershipCategoryNumber);
            return View(member);
        }

        // Edit/Update a Member record from the database
        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("MemberNumber,MembershipCategoryNumber,MembershipLastName,MembershipFirstName,MembershipAddress,MemberDOB")] Member member)
        {
            if (id != member.MemberNumber)
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
                _databasecontext.Update(member);
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

            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription", member.MembershipCategoryNumber);

            return View(member);
        }
        // Confirmaton in a Member Deletion
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberToUpdate = await _databasecontext.Members.Include(p => p.MembershipCategory).SingleOrDefaultAsync(s => s.MemberNumber == id);
            return View(memberToUpdate);
        }

        // Delete Member record from the database 
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var member = await _databasecontext.Members.FindAsync(id);
            if (member == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Members.Remove(member);
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
