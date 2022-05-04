using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class CastMemberController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public CastMemberController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.CastMembers.Include(p => p.DVDTitle).Include(p => p.Actor);
            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName");
            ViewData["ActorNumber"] = new SelectList(_databasecontext.Actors, "ActorNumber", "ActorSurname");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CastMemberNumber,DVDNumber,ActorNumber")] CastMember castmember)

        {
            try
            {


                _databasecontext.Add(castmember);
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


            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", castmember.DVDNumber);
            ViewData["ActorNumber"] = new SelectList(_databasecontext.Actors, "ActorNumber", "ActorSurname", castmember.ActorNumber);
            return View(castmember);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var castmember = await _databasecontext.CastMembers.SingleOrDefaultAsync(s => s.CastMemberNumber == id);
            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", castmember.DVDNumber);
            ViewData["ActorNumber"] = new SelectList(_databasecontext.Actors, "ActorNumber", "ActorSurname", castmember.ActorNumber);
            return View(castmember);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("CastMemberNumber,DVDNumber,ActorNumber")] CastMember castmember)
        {
            if (id != castmember.CastMemberNumber)
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
                _databasecontext.Update(castmember);
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

            ViewData["DVDNumber"] = new SelectList(_databasecontext.DVDTitles, "DVDNumber", "DVDTitleName", castmember.DVDNumber);
            ViewData["ActorNumber"] = new SelectList(_databasecontext.Actors, "ActorNumber", "ActorSurname", castmember.ActorNumber);

            return View(castmember);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var castmemberToUpdate = await _databasecontext.CastMembers.Include(p => p.DVDTitle).Include(p => p.Actor).SingleOrDefaultAsync(s => s.CastMemberNumber == id);
            return View(castmemberToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var castmember = await _databasecontext.CastMembers.FindAsync(id);
            if (castmember == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.CastMembers.Remove(castmember);
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
