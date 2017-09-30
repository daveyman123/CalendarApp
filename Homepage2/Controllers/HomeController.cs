using Homepage2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Homepage2.Controllers
{
    public class HomeController : Controller
    {
        private DefaultConnection _context;

        public HomeController()
        {
            _context = new DefaultConnection();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult New()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Create(Links home)
        {
            home.userID = User.Identity.GetUserId();
            _context.Links.Add(home);
            _context.SaveChanges();
            return RedirectToAction("Index", "home");
        }
        [Authorize]
        public ActionResult Index(string searchString)
        {
            var userID = User.Identity.GetUserId();
            var Links = (from l in _context.Links
                         where l.userID == userID
                         select l).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                Links = Links.Where(s => s.Name.Contains(searchString)).ToList();
            }

            var Name = _context.Links.ToList();
            return View(Links);

        }

        public ActionResult Remove()
        {

            var userID = User.Identity.GetUserId();
            //IList<Links> links = _context.Links);
            var Links = (from l in _context.Links
                         where l.userID == userID
                         select l).ToList();


            return View(Links);


        }

        [HttpPost]
        public ActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Links theCheese = _context.Links.Single(c => c.Id == cheeseId);
                _context.Links.Remove(theCheese);
            }

            _context.SaveChanges();

            return Redirect("/");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}