using System;
using System.Collections.Generic;
using System.Linq;
using OpenMicChicago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OpenMicChicago.Controllers {
    public class VenueController : Controller {

        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public VenueController (MyContext context) {
            dbContext = context;
        }
        private User userInSession {
            get { return dbContext.Users.FirstOrDefault (u => u.UserID == HttpContext.Session.GetInt32 ("UserID")); }
        }
        private void viewBagVenues(){
            ViewBag.Venues = dbContext.Venues;
        }
        

        [HttpGet]
        [Route ("Venues")]
        public IActionResult Venues () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login","Home");
            }
            var venues = dbContext.Venues.Include (v => v.Creator).Include(v=>v.OpenMics).OrderBy(a=>a.Name);
            return View ("Venues",venues);
        }

        

        

        [HttpGet]
        [Route ("/Venue/New")]
        public IActionResult VenueNew () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login","Home");
            }
            viewBagVenues();
            return View ("VenueNew");
        }

        [HttpPost]
        [Route ("/Venue/New/Create")]
        public IActionResult VenueNewCreate (Models.Venue venue) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login","Home");
            }
            viewBagVenues();
            
            
            
            

            if (ModelState.IsValid) {
                venue.Creator = userInSession;
                dbContext.Venues.Add (venue);
                dbContext.SaveChanges ();
                var venueID = dbContext.Venues.OrderByDescending (w => w.CreatedAt).FirstOrDefault ().VenueID;
                return Redirect($"/Venue/{venueID}");
                
            }
            return View ("VenueNew", venue);
        }

        [HttpGet]
        [Route ("Venue/{actID}/Delete")]
        public IActionResult VenueDelete (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            var Venue = dbContext.Venues.FirstOrDefault(w=>w.VenueID==actID);
            dbContext.Venues.Remove(Venue);
            dbContext.SaveChanges();
            return RedirectToAction ("Venues","Venue");
        }

        

        [HttpGet]
        [Route ("Venue/{venueID}")]
        public IActionResult VenueById (int venueID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            var venue = dbContext.Venues.Include(a=>a.Creator).Include(v=>v.OpenMics).FirstOrDefault(a=>a.VenueID==venueID);
            

            return View ("VenueById",venue);
        }

        
    }

}