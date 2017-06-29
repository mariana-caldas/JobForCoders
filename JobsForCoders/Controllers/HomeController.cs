using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JobsForCoders.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.IO;

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
                    jp.Position.Contains(search.What) ||
                    jp.Description.Contains(search.What) ||
                    jp.Name.Contains(search.What)).ToList();



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
                return RedirectToAction("Index", "JobPostings");
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
                && js.Password == login.Password)
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

        public async Task<ActionResult> Apply(int JobID)
        {
            var jobSeekerId = (int)Session["LoggedUserID"];

            var application = new Models.Application();
            application.JobID = JobID;
            application.JobSeekerID = jobSeekerId;
            application.Application_Date = DateTime.Now;

            db.Applications.Add(application);
            db.SaveChanges();

            var jobPosting = db.JobPostings
                .Include(e => e.Employer)
                .Where(e => e.JobID == JobID)
                .FirstOrDefault();

            var employer = jobPosting.Employer;

            var jobSeeker = db.JobSeekers.FirstOrDefault(js => js.JobSeekerID == jobSeekerId);

            //Email
            var body = "<p>Hello " + employer.Name + ":</p><br/><p><b>" + jobSeeker.Name + "</b> ,who is from <i>" + jobSeeker.City + "," + "</i> have just applied for the <strong>" + jobPosting.Position + "</strong> position.</p>";

            var message = new MailMessage();
            message.To.Add(new MailAddress(employer.Email));  // replace with valid value 
            message.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
            message.Subject = "New job application from #JobForCoders!";
            message.Body = body;
            message.IsBodyHtml = true;

            //To send an attachment
            var fileName = jobSeeker.CV;
            var file = Path.GetFileName(jobSeeker.CV);
            var filePath = Path.Combine(Server.MapPath("~/Uploads/"), file);

            Attachment attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.FileName = fileName;
            disposition.Size = new FileInfo(filePath).Length;
            disposition.DispositionType = DispositionTypeNames.Attachment;

            message.Attachments.Add(attachment);
            //end attachment

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "jobforcoders@hotmail.com",  // replace with valid value
                    Password = "555555Jc"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }

            return View();
        }

    }
}