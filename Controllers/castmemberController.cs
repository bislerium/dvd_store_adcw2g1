using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dvd_store_adcw2g1.Controllers
{
    public class castmemberController : Controller
    {
        private readonly DatabaseContext _databasecontext;

        public castmemberController(DatabaseContext context)
        {
            _databasecontext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _databasecontext.CastMembers.ToListAsync());
        }
    }
}
