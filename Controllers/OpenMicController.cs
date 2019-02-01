using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenMicChicago.Helpers;
using OpenMicChicago.Models;

namespace OpenMicChicago.Controllers {
    public class OpenMicController : Controller {

        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public OpenMicController (MyContext context) {
            dbContext = context;
        }
        private User userInSession {
            get { return dbContext.Users.FirstOrDefault (u => u.UserID == HttpContext.Session.GetInt32 ("UserID")); }
        }
        private void viewBagVenues () {
            ViewBag.Venues = dbContext.Venues.OrderBy (v => v.Name);
        }

        [HttpGet]
        [Route ("Home")]
        public IActionResult Home (string searchString, DateTime searchTime, int? searchGenre, string searchType, string searchAddress, int? searchDistance, string searchFavorites, string searchMyOpenMics) {

            ViewBag.Genres = dbContext.Genres.Include (g => g.OpenMics).OrderBy (g => g.Name);

            var openMics = dbContext.OpenMics.Include(o=>o.Venue).Include (o => o.Likes).ThenInclude (r => r.User).Include (o => o.Creator).Include (o => o.Genres).ThenInclude (omg => omg.Genre).OrderBy (o => o.DateTime).Where (o => o.DateTime > DateTime.Now);
            if (!String.IsNullOrEmpty (searchString)) {
                ViewBag.searchString = searchString;
                openMics = openMics.Where (o => o.Title.Contains (searchString) || o.Venue.Name.Contains (searchString) || o.Description.Contains (searchString));
            }
            if (searchTime != DateTime.MinValue) {
                ViewBag.searchTime = searchTime.ToString ("yyyy-MM-ddTHH:mm");
                openMics = openMics.Where (o => o.DateTime <= searchTime && o.EndDateTime >= searchTime);
            }
            if (searchGenre.HasValue) {
                ViewBag.searchGenre = searchGenre;
                openMics = openMics.Where (o => o.Genres.Exists (omg => omg.GenreID == (int) searchGenre));
            }
            if (!String.IsNullOrEmpty (searchType)) {
                ViewBag.searchType = searchType;
                openMics = openMics.Where (o => o.Type.Contains (searchType));
            }

            if (!String.IsNullOrEmpty (searchAddress)) {
                ViewBag.searchAddress = searchAddress;

            }
            if (searchDistance.HasValue) {
                ViewBag.searchDistance = searchDistance;

            }
            if (!String.IsNullOrEmpty (searchAddress) && searchDistance.HasValue) {
                double latitude, longitude;
                Helpers.HelperClass.getLatLong (searchAddress, out latitude, out longitude);
                var center = new Coordinates (latitude, longitude);
                
                foreach (var om in openMics) {
                    if (center.DistanceTo (new Coordinates (om.Venue.Latitude, om.Venue.Longitude)) > searchDistance) {
                        System.Console.WriteLine($"greater than {searchDistance} mi");
                        openMics=openMics.Where(o=>o.OpenMicID!=om.OpenMicID);
                        
                    }
                }
            }
            if (!String.IsNullOrEmpty (searchFavorites)){
                ViewBag.searchFavorites=searchFavorites;
                openMics=openMics.Where(o=>o.Likes.Any(l=>l.UserID==HttpContext.Session.GetInt32 ("UserID")));
            }
            if (!String.IsNullOrEmpty (searchMyOpenMics)){
                ViewBag.searchMyOpenMics=searchMyOpenMics;
                openMics=openMics.Where(O=>O.Creator.UserID==HttpContext.Session.GetInt32 ("UserID"));
            }



            return View ("Home", openMics);
        }

        [HttpGet]
        [Route ("/OpenMic/New")]
        public IActionResult OpenMicNew () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();
            return View ("OpenMicNew");
        }

        [HttpPost]
        [Route ("/OpenMic/New/Create")]
        public IActionResult OpenMicNewCreate (Models.OpenMic openMic) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();

            // if (dbContext.OpenMics.Where (a => a.Creator.UserID == HttpContext.Session.GetInt32 ("UserID")).Where (x => x.DateTime < openMic.EndDateTime && openMic.DateTime < x.EndDateTime).Count () > 0) {
            //     ModelState.AddModelError ("DateTime", "Scheduling Conflict");
            //     return View ("OpenMicNew", openMic);
            // }

            if (ModelState.IsValid) {
                openMic.Creator = userInSession;
                dbContext.OpenMics.Add (openMic);
                dbContext.SaveChanges ();
                var openMicID = dbContext.OpenMics.OrderByDescending (w => w.CreatedAt).FirstOrDefault ().OpenMicID;
                return Redirect ($"/OpenMic/{openMicID}");

            }
            return View ("OpenMicNew", openMic);
        }

        [HttpGet]
        [Route ("OpenMic/{openMicID}/Delete")]
        public IActionResult OpenMicDelete (int openMicID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var OpenMic = dbContext.OpenMics.FirstOrDefault (w => w.OpenMicID == openMicID);
            dbContext.OpenMics.Remove (OpenMic);
            dbContext.SaveChanges ();
            return RedirectToAction ("Home", "OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{openMicID}/Like")]
        public IActionResult Like (int openMicID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var act = dbContext.OpenMics.FirstOrDefault (w => w.OpenMicID == openMicID);
            var userInDb = dbContext.Users.FirstOrDefault (x => x.UserID == HttpContext.Session.GetInt32 ("UserID"));
            var like = new Like () {
                OpenMic = act,
                User = userInDb,
            };
            dbContext.Likes.Add (like);
            dbContext.SaveChanges ();

            return RedirectToAction ("Home", "OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{openMicID}/unLike")]
        public IActionResult unLike (int openMicID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }

            var like = dbContext.Likes.FirstOrDefault (r => r.UserID == HttpContext.Session.GetInt32 ("UserID") && r.OpenMicID == openMicID);
            dbContext.Likes.Remove (like);
            dbContext.SaveChanges ();

            return RedirectToAction ("Home", "OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{openMicID}")]
        public IActionResult OpenMicById (int openMicID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var openMic = dbContext.OpenMics.Include (o => o.Genres).ThenInclude (omg => omg.Genre).Include (a => a.Likes).ThenInclude (r => r.User).Include (a => a.Creator).Include (o => o.Venue).FirstOrDefault (a => a.OpenMicID == openMicID);
            ViewBag.UnusedGenres = dbContext.Genres.Include (g => g.OpenMics).Where (g => g.OpenMics.All (omg => omg.OpenMicID != openMicID)).OrderBy (g => g.Name);
            var openMics = dbContext.OpenMics.Include (a => a.Likes).ThenInclude (r => r.User).Include (a => a.Creator).OrderBy (a => a.DateTime).Where (a => a.DateTime > DateTime.Now);
            if (openMics.Where (x => x.DateTime < openMic.EndDateTime && openMic.DateTime < x.EndDateTime && x.Likes.Where (r => r.UserID == HttpContext.Session.GetInt32 ("UserID")).Count () > 0).Count () > 0) {
                ViewBag.scheduleConflict = true;
            } else {
                ViewBag.scheduleConflict = false;
            }

            return View ("OpenMicById", openMic);
        }

        [HttpPost]
        [Route ("/OpenMic/{openMicID}/AddGenre")]
        public IActionResult AddGenre (int openMicID, int genreID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var openMic = dbContext.OpenMics.FirstOrDefault (om => om.OpenMicID == openMicID);
            var genre = dbContext.Genres.FirstOrDefault (g => g.GenreID == genreID);
            var omg = new OpenMicGenre () {
                OpenMic = openMic,
                Genre = genre,
            };
            dbContext.OpenMicGenres.Add (omg);
            dbContext.SaveChanges ();
            return RedirectToAction ("OpenMicByID", openMicID);
        }

        [HttpGet]
        [Route ("/OpenMic/{openMicID}/RemoveGenre/{genreID}")]
        public IActionResult RemoveGenre (int openMicID, int genreID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            var omg = dbContext.OpenMicGenres.FirstOrDefault (o => o.OpenMicID == openMicID && o.GenreID == genreID);
            dbContext.Remove (omg);
            dbContext.SaveChanges ();
            return RedirectToAction ("OpenMicByID", openMicID);
        }

        [HttpGet]
        [Route ("OpenMic/{openMicID}/Edit")]
        public IActionResult OpenMicEdit (int openMicID) {

            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();
            var openMic = dbContext.OpenMics.Include (a => a.Creator).Include (v => v.Venue).FirstOrDefault (a => a.OpenMicID == openMicID);
            if (openMic.Creator.UserID != HttpContext.Session.GetInt32 ("UserID")) {
                return RedirectToAction ("OpenMicById", openMicID);
            }

            return View ("OpenMicEdit", openMic);
        }

        [HttpPost]
        [Route ("/OpenMic/{openMicID}/Update")]
        public IActionResult OpenMicUpdate (OpenMic openMic, int openMicID) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login", "Home");
            }
            viewBagVenues ();
            if (ModelState.IsValid) {
                openMic.OpenMicID = openMicID;

                openMic.Creator = dbContext.Users.FirstOrDefault (u => u.UserID == (int) HttpContext.Session.GetInt32 ("UserID"));
                dbContext.OpenMics.Update (openMic);
                dbContext.SaveChanges ();
                return RedirectToAction ("OpenMicById", openMicID);
            }
            return View ("OpenMicEdit", openMic);

        }

    }

}