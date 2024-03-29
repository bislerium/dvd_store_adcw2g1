﻿using dvd_store_adcw2g1.Models;
using Microsoft.AspNetCore.Mvc;

namespace dvd_store_adcw2g1.Controllers
{
    public class LoginController : Controller
    {

        private readonly DatabaseContext databaseContext;

        public LoginController(DatabaseContext db)
        {
            databaseContext = db;
        }

        public IActionResult Success()
        {
            return View();
        }


        public IActionResult Index()
        {
            return View();
        }


        //For Logging into the Application
        public IActionResult Login(string username, string userpassword)
        {


            var blog = databaseContext.Users
       .Where(b => b.UserName == username).Where(c => c.UserPassword == userpassword).FirstOrDefault();


            var data = blog;



            if (data.UserName == username && data.UserPassword == userpassword)
            {
                return View("Success");
            }
            else
            {
                return View("Index");
            }

        }

    }
}
