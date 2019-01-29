using System;
using System.Collections.Generic;
using System.Linq;
using OpenMicChicago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private void viewBagVenues(){
            ViewBag.Venues = dbContext.Venues;
        }
        

        [HttpGet]
        [Route ("Home")]
        public IActionResult Home () {
            
            var openMics = dbContext.OpenMics.Include (a => a.Likes).ThenInclude (r => r.User).Include (a => a.Creator).OrderBy(a=>a.DateTime).Where(a=>a.DateTime>DateTime.Now);
            return View ("Home",openMics);
        }

        

        

        [HttpGet]
        [Route ("/OpenMic/New")]
        public IActionResult OpenMicNew () {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login","Home");
            }
            viewBagVenues();
            return View ("OpenMicNew");
        }

        [HttpPost]
        [Route ("/OpenMic/New/Create")]
        public IActionResult OpenMicNewCreate (Models.OpenMic openMic) {
            if (HttpContext.Session.GetInt32 ("UserID") == null) {
                return RedirectToAction ("Login","Home");
            }
            viewBagVenues();
            
            
            
            if (dbContext.OpenMics.Where(a=>a.Creator.UserID==HttpContext.Session.GetInt32 ("UserID")).Where(x=>x.DateTime < openMic.EndDateTime && openMic.DateTime< x.EndDateTime).Count()>0){
                    ModelState.AddModelError("DateTime","Scheduling Conflict");
                    return View ("OpenMicNew",openMic);
                }

            if (ModelState.IsValid) {
                openMic.Creator = userInSession;
                dbContext.OpenMics.Add (openMic);
                dbContext.SaveChanges ();
                var openMicID = dbContext.OpenMics.OrderByDescending (w => w.CreatedAt).FirstOrDefault ().OpenMicID;
                return Redirect($"/OpenMic/{openMicID}");
                
            }
            return View ("OpenMicNew", openMic);
        }

        [HttpGet]
        [Route ("OpenMic/{actID}/Delete")]
        public IActionResult OpenMicDelete (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            var OpenMic = dbContext.OpenMics.FirstOrDefault(w=>w.OpenMicID==actID);
            dbContext.OpenMics.Remove(OpenMic);
            dbContext.SaveChanges();
            return RedirectToAction ("Home","OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{actID}/Like")]
        public IActionResult Like (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            var act = dbContext.OpenMics.FirstOrDefault(w=>w.OpenMicID==actID);
            var userInDb = dbContext.Users.FirstOrDefault(x=>x.UserID == HttpContext.Session.GetInt32 ("UserID"));
            var like = new Like(){
                OpenMic = act,
                User = userInDb,
            };
            dbContext.Likes.Add(like);
            dbContext.SaveChanges();

            return RedirectToAction ("Home","OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{actID}/unLike")]
        public IActionResult unLike (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            
            
            var like = dbContext.Likes.FirstOrDefault(r=>r.UserID==HttpContext.Session.GetInt32 ("UserID") && r.OpenMicID==actID );
            dbContext.Likes.Remove(like);
            dbContext.SaveChanges();

            return RedirectToAction ("Home","OpenMic");
        }

        [HttpGet]
        [Route ("OpenMic/{actID}")]
        public IActionResult OpenMicById (int actID) {
            if (HttpContext.Session.GetInt32 ("UserID")==null)
            {
                return RedirectToAction ("Login","Home");
            }
            var openMic = dbContext.OpenMics.Include(a=>a.Likes).ThenInclude(r=>r.User).Include(a=>a.Creator).FirstOrDefault(a=>a.OpenMicID==actID);
            var openMics = dbContext.OpenMics.Include (a => a.Likes).ThenInclude (r => r.User).Include (a => a.Creator).OrderBy(a=>a.DateTime).Where(a=>a.DateTime>DateTime.Now);
            if( openMics.Where(x=>x.DateTime < openMic.EndDateTime && openMic.DateTime< x.EndDateTime && x.Likes.Where(r=>r.UserID ==HttpContext.Session.GetInt32("UserID")).Count()>0).Count()>0){
                ViewBag.scheduleConflict = true;
            }
             else
             {
                 ViewBag.scheduleConflict = false;
             }

            return View ("OpenMicById",openMic);
        }

        
    }

}