using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dvd_store_adcw2g1.Models.ViewModels;
using System.Data;
using System.Web;

namespace dvd_store_adcw2g1.Controllers
{
    public class ActorController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public ActorController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            //if (HttpContext.Session.GetString("role") == null)
            //{ // for controller
            //    return RedirectToAction(controllerName: "Home", actionName: "Index");
            //}
            //else
            //{
                
                var actor = from m in _databasecontext.Actors
                            select m;
                if (!String.IsNullOrEmpty(searchString))
                {
                    actor = from m in _databasecontext.Actors.Where(a => a.ActorSurname.Contains(searchString)) select m;
                }
                return View(await actor.ToListAsync());
            //}

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor)
  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(actor);
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
            return View(actor);
        }

        public async Task<IActionResult> Details(int id)
        {

            var query = from a in _databasecontext.Actors
                        join cm in _databasecontext.CastMembers
                        on a.ActorNumber equals cm.ActorNumber
                        join dt in _databasecontext.DVDTitles
                        on cm.DVDNumber equals dt.DVDNumber
                        where a.ActorNumber == id
                        select dt;

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var actorToUpdate = await _databasecontext.Actors.SingleOrDefaultAsync(s => s.ActorNumber == id);
            return View(actorToUpdate);
        }

        public async Task<IActionResult> DVDOnShelves(string searchString)
        {
            var DVDnotLoaned = from dt in _databasecontext.DVDTitles
                               join cm in _databasecontext.CastMembers on
                               dt.DVDNumber equals cm.DVDNumber
                               join dc in _databasecontext.DVDCopies.Where(c => _databasecontext.Loans.Any(l => (l.DateReturned != null)))
                               on dt.DVDNumber equals dc.DVDNumber
                               join a in _databasecontext.Actors
                               .Where(x => x.ActorSurname.Contains(searchString))
                               on cm.ActorNumber equals a.ActorNumber
                               group new { dt, cm, dc } by new { dt.DVDNumber, dt.DVDTitleName, a.ActorSurname }
                                   into grp
                               select new DVDOnShelves
                               {
                                   DVDNumber = grp.Key.DVDNumber,
                                   DVDCount = grp.Count(),
                                   ActorSurname = grp.Key.ActorSurname,
                                   DVDTitle = grp.Key.DVDTitleName,
                               };
            return View(await DVDnotLoaned.ToListAsync());
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _databasecontext.Actors.FirstOrDefaultAsync(s => s.ActorNumber == id);
            if (await TryUpdateModelAsync<Actor>(
                studentToUpdate,
                "",
                s => s.ActorSurname, s => s.ActorFirstName))
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
            return View(studentToUpdate);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actorToUpdate = await _databasecontext.Actors.SingleOrDefaultAsync(s => s.ActorNumber == id);
            return View(actorToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var actor = await _databasecontext.Actors.FindAsync(id);
            if (actor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Actors.Remove(actor);
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
