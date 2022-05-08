using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var actors = from s in _databasecontext.CastMembers.Include(p => p.DVDTitle).Include(p => p.Actor)
                           select s;

            actors = actors.Where(s => s.Actor.ActorNumber.Equals(id));
            return View(await actors.ToListAsync());
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var actorToUpdate = await _databasecontext.Actors.SingleOrDefaultAsync(s => s.ActorNumber == id);
            return View(actorToUpdate);
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
