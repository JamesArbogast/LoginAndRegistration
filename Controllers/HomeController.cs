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
            if(ModelState.IsValid == false)
            {
                return View("Index"); 
            }
            if(dbContext.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "This Email is already in user!");
            }

            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            // HttpContext.Session.SetInt32("userId", newUser.UserId);
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return RedirectToAction("ViewLogin");
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
        public IActionResult LoginUser(LoginUser userLogin)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userLogin.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Success");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userLogin, userInDb.Password, userLogin.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Password is incorrect.");
                }
            }
            return RedirectToAction("NewUser", userLogin);
        }

        public IActionResult ViewSuccess()
        {
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
