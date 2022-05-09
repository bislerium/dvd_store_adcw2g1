using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace dvd_store_adcw2g1.Controllers
{
    public class DVDTitleController : Controller
    {

        private readonly DatabaseContext _databasecontext;
        private readonly TextInfo _textInfo;

        public DVDTitleController(DatabaseContext context)
        {
            _databasecontext = context;
            _textInfo = new CultureInfo("en-US", false).TextInfo;
        }
        public async Task<IActionResult> Index()
        {
            var databasecontext = _databasecontext.DVDTitles.Include(p => p.Producer).Include(p => p.DVDCategory).Include(p => p.Studio);
            ViewData["ProducerNumber"] = await (from p in _databasecontext.Producers
                                          select p.ProducerName).ToListAsync();
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription");
            ViewData["StudioNumber"] = await (from s in _databasecontext.Studios
                                              select s.StudioName).ToListAsync();
            return View(await databasecontext.ToListAsync());
        }
        private String ToTitleCase(String @string) => _textInfo.ToTitleCase(@string.Trim());
        [HttpPost]
        public async Task<IActionResult> Create([Bind("DVDTitleName,Producer,DVDCategory,Studio,Actors,DateReleased,StandardCharge,PenaltyCharge")] NewDVDTiTle dvdTitle)
        {
            Console.WriteLine(dvdTitle);
            String producer = ToTitleCase(dvdTitle.Producer);
            String studio = ToTitleCase(dvdTitle.Studio);
            List<String> actors = dvdTitle.Actors.ConvertAll(value => ToTitleCase(value));
            try
            {
                var producerRecord = (await _databasecontext.Producers.FirstOrDefaultAsync(p => p.ProducerName == producer))
                    ?? (await _databasecontext.Producers.AddAsync(new Producer() { ProducerName = producer })).Entity;
                var studioRecord = (await _databasecontext.Studios.FirstOrDefaultAsync(s => s.StudioName == studio))
                    ?? (await _databasecontext.Studios.AddAsync(new Studio() { StudioName = studio })).Entity;
                var dvdCategoryRecord = await _databasecontext.DVDCategories.FirstOrDefaultAsync(c => c.CategoryNumber == dvdTitle.DVDCategory);
                var dvdTitleRecord = (await _databasecontext.DVDTitles.AddAsync(new DVDTitle()
                {
                    DVDTitleName = dvdTitle.DVDTitleName,
                    Producer = producerRecord,
                    Studio = studioRecord,
                    DVDCategory = dvdCategoryRecord!,
                    DateReleased = dvdTitle.DateReleased,
                    StandardCharge = dvdTitle.StandardCharge,
                    PenaltyCharge = dvdTitle.PenaltyCharge,
                })).Entity;
                foreach (var actor in actors) {
                    List<String> _ = actor.Split(' ').ToList();
                    var firstName = _.First();
                    _.RemoveAt(0);
                    var lastName = String.Join(' ', _);
                    var actorRecord = await _databasecontext.Actors.FirstOrDefaultAsync(a => a.ActorFirstName == firstName && a.ActorSurname == lastName);
                    if (actorRecord == null) {
                        actorRecord = (await _databasecontext.Actors.AddAsync(new Actor() { ActorFirstName = firstName, ActorSurname = lastName })).Entity;
                    }
                    await _databasecontext.CastMembers.AddAsync(new CastMember() { DVDTitle = dvdTitleRecord, Actor = actorRecord });
                }
                await _databasecontext.SaveChangesAsync();
                Console.WriteLine(_databasecontext.DVDTitles.FindAsync(dvdTitleRecord.DVDNumber));
            }
            catch (Exception ex )
            {
                ViewBag.error = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("DVDNumber,DVDTitleName,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dvdtitle)

        //{
        //    try
        //    {
        //            _databasecontext.Add(dvdtitle);
        //            await _databasecontext.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
                
        //    }
        //    catch (DbUpdateException /* ex */)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        ModelState.AddModelError("", "Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator.");
        //    }

        //    return View(nameof(Index));
        //}

        public async Task<IActionResult> EditPost(int id)
        {
            var dvdtitleToUpdate = await _databasecontext.DVDTitles.SingleOrDefaultAsync(s => s.DVDNumber == id);
            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName", dvdtitleToUpdate.ProducerNumber);
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription", dvdtitleToUpdate.CategoryNumber);
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName", dvdtitleToUpdate.StudioNumber);
            return View(dvdtitleToUpdate);
        }


        [HttpPost, ActionName("EditPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, [Bind("DVDNumber,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dvdtitle)
        {
            if (id != dvdtitle.DVDNumber)
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
                    _databasecontext.Update(dvdtitle);
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
            
            ViewData["ProducerNumber"] = new SelectList(_databasecontext.Producers, "ProducerNumber", "ProducerName", dvdtitle.ProducerNumber);
            ViewData["CategoryNumber"] = new SelectList(_databasecontext.DVDCategories, "CategoryNumber", "CategoryDescription", dvdtitle.CategoryNumber);
            ViewData["StudioNumber"] = new SelectList(_databasecontext.Studios, "StudioNumber", "StudioName", dvdtitle.StudioNumber);
            
            return View(dvdtitle);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dvdtitleToUpdate = await _databasecontext.DVDTitles.Include(p => p.Producer).Include(p => p.DVDCategory).Include(p => p.Studio).SingleOrDefaultAsync(s => s.DVDNumber == id);
            return View(dvdtitleToUpdate);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var dvdtitle = await _databasecontext.DVDTitles.FindAsync(id);
            if (dvdtitle == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _databasecontext.DVDTitles.Remove(dvdtitle);
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
