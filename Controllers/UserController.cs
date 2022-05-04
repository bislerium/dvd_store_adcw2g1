using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class UserController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public UserController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.Users.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(user);
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
            return View(user);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var userToUpdate = await _databasecontext.Users.SingleOrDefaultAsync(s => s.UserNumber == id);
            return View(userToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userToUpdate = await _databasecontext.Users.FirstOrDefaultAsync(s => s.UserNumber == id);
            if (await TryUpdateModelAsync<User>(
                userToUpdate,
                "",
                s => s.UserName, s => s.UserType, s => s.UserPassword))
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
            return View(userToUpdate);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userToUpdate = await _databasecontext.Users.SingleOrDefaultAsync(s => s.UserNumber == id);
            return View(userToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var user = await _databasecontext.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Users.Remove(user);
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
