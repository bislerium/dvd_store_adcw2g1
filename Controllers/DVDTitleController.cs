using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.Others;
using dvd_store_adcw2g1.Models.ViewModels;
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

        public async Task<IActionResult> DVDDetails()
        {
            var query = from s in _databasecontext.CastMembers
                        join d in _databasecontext.DVDTitles
                        on s.DVDNumber equals d.DVDNumber
                        join a in _databasecontext.Actors
                        on s.ActorNumber equals a.ActorNumber
                        join p in _databasecontext.Producers
                        on d.ProducerNumber equals p.ProducerNumber
                        join st in _databasecontext.Studios
                        on d.StudioNumber equals st.StudioNumber
                        group a by d.DVDNumber into dg  
                        orderby (from dt in _databasecontext.DVDTitles
                                 where dt.DVDNumber == dg.Key
                                 select dt).FirstOrDefault()!.DateReleased
                        select new DVDActors()
                        {
                            DVDTitle = (from dt in _databasecontext.DVDTitles
                                        where dt.DVDNumber == dg.Key
                                        select dt).FirstOrDefault()!,
                            Producer = (from dt in _databasecontext.DVDTitles
                                        join p in _databasecontext.Producers
                                        on dt.ProducerNumber equals p.ProducerNumber
                                        where dt.DVDNumber == dg.Key
                                        select p.ProducerName).FirstOrDefault()!,
                            Studio = (from dt in _databasecontext.DVDTitles
                                      join s in _databasecontext.Studios
                                      on dt.StudioNumber equals s.StudioNumber
                                      where dt.DVDNumber == dg.Key
                                      select s.StudioName).FirstOrDefault()!,
                            Actors = dg.OrderBy(a => a.ActorSurname).ToList(),
                        }                        ;

            return View(await query.ToListAsync());
        }
        private String ToTitleCase(String @string) => _textInfo.ToTitleCase(@string.Trim());

        /// <summary>
        /// [FUNCTION 9] Create a new DVD title with auto-assignment/creation of producer, studio, category and actors as per data in ModelForm.
        /// </summary>
        /// <param name="dvdTitle">NewDVDTitle ModelForm with required form-data for DVDTitle record addition in Database</param>
        /// <returns>Renders Relevant View-Page</returns>
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

        /// <summary>
        /// [FUNCTION 13] Displays a list of all DVD titles in the shop where no copy of the title has been loaned in the last 31 days.
        /// </summary>
        /// <returns>Renders Relevant View-Page</returns>
        public async Task<IActionResult> UnloanedDVDs()
        {
            var loans = _databasecontext.Loans;
            var dvdCopies = _databasecontext.DVDCopies;
            var query = from c in dvdCopies
                        join l in loans
                        on c.CopyNumber equals l.CopyNumber
                        group l by c.DVDNumber into lg
                        where DateTime.Now > lg.Max(a => a.DateOut).AddDays(31)                        select lg.First().DVDCopy.DVDTitle;
            return View(await query.ToListAsync());
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
