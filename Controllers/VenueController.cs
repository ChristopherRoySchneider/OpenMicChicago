using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenMicChicago.Models;
using OpenMicChicago.Helpers;

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
        private void viewBagVenues () {
            ViewBag.Venues = dbContext.Venues;
        }
        

        [HttpGet]
        [Route ("Venues")]
        public IActionResult Venues () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var venues = dbContext.Venues.Include (v => v.Creator).Include (v => v.OpenMics).OrderBy (a => a.Name);
            return View ("Venues", venues);
        }

        [HttpGet]
        [Route ("/Venue/New")]
        public IActionResult VenueNew () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();
            return View ("VenueNew");
        }

        [HttpPost]
        [Route ("/Venue/New/Create")]
        public IActionResult VenueNewCreate (Models.Venue venue) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();

            if (ModelState.IsValid)
            {
                venue.Creator = userInSession;

                
                double latitude, longitude;
                HelperClass.getLatLong(venue.Address, out latitude, out longitude);
                venue.Latitude = latitude;
                venue.Longitude = longitude;


                dbContext.Venues.Add(venue);
                dbContext.SaveChanges();
                var venueID = dbContext.Venues.OrderByDescending(w => w.CreatedAt).FirstOrDefault().VenueID;

                return Redirect($"/Venue/{venueID}");

            }
            return View ("VenueNew", venue);
        }

        

        [HttpGet]
        [Route ("Venue/{actID}/Delete")]
        public IActionResult VenueDelete (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var Venue = dbContext.Venues.FirstOrDefault (w => w.VenueID == actID);
            dbContext.Venues.Remove (Venue);
            dbContext.SaveChanges ();
            return RedirectToAction ("Venues", "Venue");
        }

        [HttpGet]
        [Route ("Venue/{venueID}")]
        public IActionResult VenueById (int venueID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var venue = dbContext.Venues.Include (a => a.Creator).Include (v => v.OpenMics).FirstOrDefault (a => a.VenueID == venueID);

            return View ("VenueById", venue);
        }

        [HttpGet]
        [Route ("Venue/{venueID}/Edit")]
        public IActionResult VenueEdit (int venueID) {

            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }

            var venue = dbContext.Venues.Include (a => a.Creator).Include (v => v.OpenMics).FirstOrDefault (a => a.VenueID == venueID);
            if (venue.Creator.UserID != HttpContext.Session.GetInt32 ("UserID")) {
                return RedirectToAction ("VenueById", venueID);
            }

            return View ("VenueEdit", venue);
        }

        [HttpPost]
        [Route ("/Venue/{venueID}/Update")]
        public IActionResult VenueUpdate (Venue venue, int venueID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            if (ModelState.IsValid) {
                venue.VenueID = venueID;

                double latitude, longitude;
                HelperClass.getLatLong(venue.Address, out latitude, out longitude);
                venue.Latitude = latitude;
                venue.Longitude = longitude;

                venue.Creator = dbContext.Users.FirstOrDefault (u => u.UserID == (int) HttpContext.Session.GetInt32 ("UserID"));
                dbContext.Venues.Update (venue);
                dbContext.SaveChanges ();
                return RedirectToAction ("VenueById", venueID);
            }
            return View ("VenueEdit", venue);

        }

    }

}