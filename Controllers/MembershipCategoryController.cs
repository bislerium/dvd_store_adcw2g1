using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class MembershipCategoryController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public MembershipCategoryController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.MembershipCategories.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipCategory membershipcategory)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(membershipcategory);
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
            return View(membershipcategory);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var membershipcategoryToUpdate = await _databasecontext.MembershipCategories.SingleOrDefaultAsync(s => s.MembershipCategoryNumber == id);
            return View(membershipcategoryToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var membershipcategoryToUpdate = await _databasecontext.MembershipCategories.FirstOrDefaultAsync(s => s.MembershipCategoryNumber == id);
            if (await TryUpdateModelAsync<MembershipCategory>(
                membershipcategoryToUpdate,
                "",
                s => s.MembershipCategoryDescription, s => s.MembershipCategoryTotalLoans))
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
            return View(membershipcategoryToUpdate);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipcategoryToUpdate = await _databasecontext.MembershipCategories.SingleOrDefaultAsync(s => s.MembershipCategoryNumber == id);
            return View(membershipcategoryToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var membershipcategory = await _databasecontext.MembershipCategories.FindAsync(id);
            if (membershipcategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.MembershipCategories.Remove(membershipcategory);
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
