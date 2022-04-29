using dvd_store_adcw2g1.Models;

using Microsoft.AspNetCore.Mvc;

namespace dvd_store_adcw2g1.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DatabaseContext databaseContext;


        public RegisterController(DatabaseContext db)
        {
            databaseContext = db;
        }


        public IActionResult Create()
        {
           
        
            return View();
            
        }
        [HttpPost]
        public IActionResult Create(User user)
        {

            user.UserName = user.UserName;
            user.UserType = "Customer";
            user.UserPassword = user.UserPassword;

            
            databaseContext.Users.Add(user);
            databaseContext.SaveChanges();


            return View();
        }



        //public IActionResult Register(User user)
        //{
        //    user.UserName = "1231231231";
        //    user.UserType = "admin@admin.com";
        //    user.UserPassword = "asdasd";


        //    dataBaseContext.Users.Add(user);
        //    dataBaseContext.SaveChanges();
        //    return View();
        //}



    }
}
