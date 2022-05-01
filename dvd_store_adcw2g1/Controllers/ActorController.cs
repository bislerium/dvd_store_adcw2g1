﻿using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class ActorController : Controller
    {

        private readonly DatabaseContext _databasecontext;
         
        public ActorController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.Actors.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

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
            var actorToUpdate = await _databasecontext.Actors.FirstOrDefaultAsync(s => s.ActorNumber == id);
            if (await TryUpdateModelAsync<Actor>(
                actorToUpdate,
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
            return View(actorToUpdate);
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