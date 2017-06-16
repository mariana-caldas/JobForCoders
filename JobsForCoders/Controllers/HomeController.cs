using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JobsForCoders.Models;
using System.Collections.Generic;

namespace JobsForCoders.Controllers
{
    public class HomeController : Controller
    {
        private jobsforcodersEntities db = new jobsforcodersEntities();

        public ActionResult Index()
        {
            if (Session["LoggedUser"] != null)
            {
                if (Session["What"] != null)
                {
                    var search = new Search
                    {
                        What = Session["What"].ToString()
                    };
                    return View(search);
                }
            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobList(Search search)
        {
            if (Session["LoggedUser"] != null)
            {
                var exists = db.JobPostings.Include(jp => jp.Employer).Where(jp => 
                    search.What.Contains(jp.Position) || 
                    search.What.Contains(jp.Description) || 
                    search.What.Contains(jp.Name)).ToList();

                return View(exists);
            }

            Session["What"] = search.What;
            Session["Where"] = search.Where;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Logout()
        {
            Session["LoggedUser"] = null;
            Session["LoggedUserType"] = null;
            Session["LoggedUserID"] = null;
            return RedirectToAction("Index");
        }

        //Login for Employeer
        public ActionResult EmployerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployerLogin([Bind(Include = "Email,Password")] Login login)
        {
            var exists = db.Employers.Where(e =>
               e.Email == login.Email
               && e.Password == login.Password)
               .FirstOrDefault();

            if (exists != null)
            {
                Session["LoggedUser"] = exists.Name;
                Session["LoggedUserType"] = "Employers";
                Session["LoggedUserID"] = exists.EmployerID;
                return RedirectToAction("Index" , "JobPostings");
            }

            ViewBag.MessageErrorLogin = "Email or Password invalids";

            return View();
        }

        //end

        //Login for JobSeeker
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "Email,Password")] Login login)
        {
            var exists = db.JobSeekers.Where(js => 
                js.Email == login.Email 
                && js.Password==login.Password)
                .FirstOrDefault();

            if (exists != null)
            {
                Session["LoggedUser"] = exists.Name;
                Session["LoggedUserType"] = "JobSeekers";
                Session["LoggedUserID"] = exists.JobSeekerID;
                return RedirectToAction("Index");
            }

            ViewBag.MessageErrorLogin = "Email or Password invalids";

            return View();
        }
        //end

    }
}