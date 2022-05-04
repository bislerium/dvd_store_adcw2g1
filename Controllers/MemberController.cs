using dvd_store_adcw2g1.Models;
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
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.Members.Include(p => p.MembershipCategory);
            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription");
         
            return View();

        }

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

        public async Task<IActionResult> EditPost(int id)
        {
            var member = await _databasecontext.Members.SingleOrDefaultAsync(s => s.MemberNumber == id);
            ViewData["MembershipCategoryNumber"] = new SelectList(_databasecontext.MembershipCategories, "MembershipCategoryNumber", "MembershipCategoryDescription", member.MembershipCategoryNumber);
            return View(member);
        }


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

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberToUpdate = await _databasecontext.Members.Include(p => p.MembershipCategory).SingleOrDefaultAsync(s => s.MemberNumber == id);
            return View(memberToUpdate);
        }

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
