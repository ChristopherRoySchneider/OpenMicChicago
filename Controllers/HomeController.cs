using System;
using System.Collections.Generic;
using System.Linq;
using OpenMicChicago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OpenMicChicago.Controllers {
    public class HomeController : Controller {

        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController (MyContext context) {
            dbContext = context;
        }
        private User userInSession {
            get { return dbContext.Users.FirstOrDefault (u => u.UserID == HttpContext.Session.GetInt32 ("UserID")); }
        }
        // GET: /Home/
        [HttpGet]
        [Route ("Register")]
        public IActionResult Register () {
            return View ("Register");
        }

        [HttpGet]
        [Route ("")]
        public IActionResult Index () {
            return Redirect("Home");
        }

        [HttpPost ("SubmitRegistration")]
        public IActionResult SubmitRegistration (User user) {
            // Check initial ModelState
            if (ModelState.IsValid) {

                PasswordHasher<User> Hasher = new PasswordHasher<User> ();
                user.Password = Hasher.HashPassword (user, user.Password);
                // If a User exists with provided email
                if (dbContext.Users.Any (u => u.Email == user.Email)) {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError ("Email", "Email already in use!");
                    return View ("Register");
                    // You may consider returning to the View at this point
                }
                dbContext.Add (user);
                // OR dbContext.Users.Add(newUser);
                dbContext.SaveChanges ();
                User userFromDB = dbContext.Users.OrderByDescending (u => u.UserID).FirstOrDefault ();
                HttpContext.Session.SetInt32 ("UserID", userFromDB.UserID);
                HttpContext.Session.SetString ("UserName", $"{userFromDB.FirstName} {userFromDB.LastName}");
                return RedirectToAction ("Home","OpenMic");
            } else {
                return View ("Register");
            }
        }

        

        [HttpGet]
        [Route ("Logout")]
        public IActionResult Logout () {

            HttpContext.Session.Clear ();
            return RedirectToAction ("Login","Home");
        }

        [HttpGet]
        [Route ("Login")]
        public IActionResult Login () {
            return View ("Login");
        }

        [HttpPost ("SubmitLogin")]
        public IActionResult SubmitLogin (LoginUser userSubmission) {
            if (ModelState.IsValid) {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault (u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null) {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError ("Email", "Invalid Email/Password");
                    return View ("Login");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser> ();

                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword (userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0) {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError ("Email", "Invalid Email/Password");
                    return View ("Login");
                }
                HttpContext.Session.SetInt32 ("UserID", userInDb.UserID);
                HttpContext.Session.SetString ("UserName", $"{userInDb.FirstName} {userInDb.LastName}");
                return RedirectToAction ("Home","OpenMic");
            } else {
                return View ("Login");
            }
        }

        
    }

}