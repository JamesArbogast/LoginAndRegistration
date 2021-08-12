using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginAndRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LoginAndRegistration.Controllers
{
    public class HomeController : Controller
    {
        private LoginAndRegistrationContext dbContext;
        public HomeController(LoginAndRegistrationContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpPost("/users/create")]
        public IActionResult CreateUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "This Email is already in user!");
                }
            }

            if (ModelState.IsValid == false)
            {
                return View("Index"); 
            }

            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            if(HttpContext.Session.GetString("Email") == null)
            {
                HttpContext.Session.SetString("Email", "User.Email");
            }

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FirstName", newUser.FirstName);

            return View("Success");
        }

        [HttpGet("/users/{userId}")]
        public IActionResult NewUser(int userId)
        {
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            return View("NewUser", user);
        }
        [HttpGet("/login")]
        public IActionResult ViewLogin()
        {
            return View("Login");
        }
        public IActionResult Logout()
        {
            if(HttpContext.Session.GetString("Email") != null)
            {
                HttpContext.Session.Clear();
                return View("Index");
            }
            return View("Index");
        }
        public IActionResult LoginUser(LoginUser userLogin)
        {
            if(ModelState.IsValid == false)
            {
                return View("Login");
            }

            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userLogin.LoginEmail);
            
            if(userInDb == null)
            {
                ModelState.AddModelError("LoginError", "Email not found");
                return View("Login");
            }

            var hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userLogin, userInDb.Password, userLogin.LoginPassword);


            if(result == 0)
            {
                ModelState.AddModelError("LoginPassword", "Password is incorrect.");
                return View("Login");
            }

            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            HttpContext.Session.SetString("FirstName", userInDb.FirstName);
            
            return RedirectToAction("ViewSuccess");
        }

        public IActionResult ViewSuccess()
        {
            if(HttpContext.Session.GetString("Email") == null)
            {
                return View("Index");
            }
            return View("Success");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
