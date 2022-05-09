using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class ProducerController : Controller
    {

        private readonly DatabaseContext _databasecontext;

        public ProducerController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.Producers.ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            return View();

        }

        // Create a producer record in database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producer producer)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databasecontext.Add(producer);
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
            return View(producer);
        }

        // Filled-in form data for modalshhet to Edit Producer
        public async Task<IActionResult> EditPost(int id)
        {
            var producerToUpdate = await _databasecontext.Producers.SingleOrDefaultAsync(s => s.ProducerNumber == id);
            return View(producerToUpdate);
        }


        // Update the Producer record in the database
        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var producerToUpdate = await _databasecontext.Producers.FirstOrDefaultAsync(s => s.ProducerNumber == id);
            if (await TryUpdateModelAsync<Producer>(
                producerToUpdate,
                "",
                s => s.ProducerName))
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
            return View(producerToUpdate);
        }

        // Confirmation for Producer record deletion
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producerToUpdate = await _databasecontext.Producers.SingleOrDefaultAsync(s => s.ProducerNumber == id);
            return View(producerToUpdate);
        }

        // Delete the producer record from the database
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var producer = await _databasecontext.Producers.FindAsync(id);
            if (producer == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.Producers.Remove(producer);
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
