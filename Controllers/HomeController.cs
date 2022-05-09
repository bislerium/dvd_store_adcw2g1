using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace dvd_store_adcw2g1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DatabaseContext databaseContext;

        // Logging out from the application
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("role");

            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            databaseContext = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("role") == null)
            { // for controller
                return View();
            }
            else
            {
                return RedirectToAction(controllerName: "DVDTitle", actionName: "Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // For logging into the application
        public IActionResult Login(string username, string userpassword)
        {

            var blog = databaseContext.Users
            .Where(b => b.UserName == username).Where(c => c.UserPassword == userpassword).FirstOrDefault();

            var data = blog;

            if (data != null && data.UserName == username && data.UserPassword == userpassword)
            {
                HttpContext.Session.SetString("role", data.UserType);
                HttpContext.Session.SetString("userid", data.UserNumber.ToString());
                //return RedirectToAction(controllerName: "actor", actionName: "Index");
                return RedirectToAction(controllerName: "DVDTitle", actionName: "Index");
            }
            else
            {
                return View("Index");
            }

        }

        // Create a User Record in the database
        public IActionResult Create(User user)
        {

            user.UserName = user.UserName;
            user.UserType = "Admin";
            user.UserPassword = user.UserPassword;


            databaseContext.Users.Add(user);
            databaseContext.SaveChanges();


            return View("Index");
        }

    }
}