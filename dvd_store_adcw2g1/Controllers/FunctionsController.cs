using dvd_store_adcw2g1.Models;
using dvd_store_adcw2g1.Models.ViewModels;
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

        public async Task<IActionResult> Function2()
        {
            var person = (from e in _databasecontext.DVDTitles
                          join p in _databasecontext.CastMembers
                          on e.DVDNumber equals p.DVDNumber
                          join s in _databasecontext.DVDCopies.Where(c => _databasecontext.Loans.Any(l => (c.CopyNumber == l.CopyNumber && l.DateReturned != null)))
                          on e.DVDNumber equals s.DVDNumber
                          join t in _databasecontext.Actors
                          on p.ActorNumber equals t.ActorNumber 
                          select new  DVDonShelves { DVDTitle = e, Actor = t }
                          );

            //ViewData["person"] = await person.ToListAsync();
            return View(await person.ToListAsync());
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

        public async Task<IActionResult> Function5(int searchString)
        {
            var databasecontext = from s in _databasecontext.Loans.Include(p => p.LoanType).Include(p => p.DVDCopy).Include(p => p.Member) select s;

            if (!String.IsNullOrEmpty(searchString.ToString()))
            {
                databasecontext = databasecontext.Where(s => s.DVDCopy.CopyNumber.ToString().Contains(searchString.ToString()));

            }

            return View(await databasecontext.ToListAsync());
        }




        public async Task<IActionResult> Test(string searchString)
        {
            var person = (from e in _databasecontext.DVDTitles
                          join p in _databasecontext.DVDCopies
                          on e.DVDNumber equals p.DVDNumber
                        
                          select new { DVDCopy = p.DatePurchased }
                          ).ToList();


            return Json(person);
        }



        public async Task<IActionResult> Function6(int searchString)
        {
            var person = (from e in _databasecontext.DVDTitles
                          join p in _databasecontext.DVDCategories
                          on e.CategoryNumber equals p.CategoryNumber
                          join s in _databasecontext.DVDCopies
                          on e.DVDNumber equals s.DVDNumber
                          join t in _databasecontext.Loans
                          on s.CopyNumber equals t.CopyNumber
                          join w in _databasecontext.LoanTypes
                          on t.LoanTypeNumber equals w.LoanTypeNumber
                          select new { DVDTitle = e, DVDCategory = p, DVDCopy = s, Loan = t, LoanType = w }
                          );

            return Json(person);

        }



    }
    
}
