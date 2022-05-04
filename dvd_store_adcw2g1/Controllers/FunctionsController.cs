using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace dvd_store_adcw2g1.Controllers
{
    public class FunctionsController : Controller
    {
        private readonly DatabaseContext _databasecontext;

        public FunctionsController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Function1(string searchString)
            {

                var students = from s in _databasecontext.CastMembers.Include(p => p.DVDTitle).Include(p => p.Actor)
                               select s;


                if (!String.IsNullOrEmpty(searchString))
                {
                    students = students.Where(s => s.Actor.ActorSurname.Contains(searchString));

                }

                return View(await students.ToListAsync());
            }



        public async Task<IActionResult> Function3(string searchString)
        {
            var databasecontext = from s in _databasecontext.Loans.Include(p => p.DVDCopy).Include(p => p.Member).Include(p => p.DVDCopy.DVDTitle) select s;

            var today = DateTime.Today.AddDays(-31);

            databasecontext = databasecontext.Where(s => s.DateOut >= today);

            if (!String.IsNullOrEmpty(searchString))
            {
                databasecontext = databasecontext.Where(s => s.Member.MembershipLastName.Contains(searchString));

            }

            return View(await databasecontext.ToListAsync());
        }


        public async Task<IActionResult> Function4(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var students = from s in _databasecontext.CastMembers.Include(p => p.DVDTitle.Producer).Include(p => p.DVDTitle.Studio).Include(p => p.Actor)
                           select s;


            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderBy(s => s.Actor.ActorSurname);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.DVDTitle.DateReleased);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.DVDTitle.DateReleased);
                    break;
                default:
                    students = students.OrderBy(s => s.Actor.ActorSurname);
                    break;
            }

            return View(await students.ToListAsync());
        }



    }
    
}
