using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobsForCoders.Models;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using WebGrease.Activities;

namespace JobsForCoders.Controllers
{
    public class EmployersController : Controller
    {
        private jobsforcodersEntities db = new jobsforcodersEntities();

        // GET: Employers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        // GET: Employers/Create
        public ActionResult Create()
        {
            return View();
        }

        public enum FileType
        {
            Avatar = 1, Photo
        }

        // POST: Employers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EmployerID,Name,Address,City,Email,Password,Cellphone,Operator,Buzz_Words")] Employer employer, HttpPostedFileBase Logo, HttpPostedFileBase History)
        {
            if (ModelState.IsValid)
            {
                if (History != null && History.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(History.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    History.SaveAs(path);

                    employer.History = fileName;
                }

                if (Logo != null && Logo.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Logo.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    Logo.SaveAs(path);

                    employer.Logo = fileName;
                }

                db.Employers.Add(employer);
                db.SaveChanges();

                //Determine the Cellphone Operator
                var operatorValue = employer.Operator;
                if (operatorValue == "Virgin")
                {
                    employer.Cellphone = employer.Cellphone + "@vmobile.ca";
                }
                if (operatorValue == "Fido" || operatorValue == "Microcell")
                {
                    employer.Cellphone = employer.Cellphone + "@fido.ca";
                }
                if (operatorValue == "Rogers")
                {
                    employer.Cellphone = employer.Cellphone + "@fido.ca";
                }
                if (operatorValue == "Bell")
                {
                    employer.Cellphone = employer.Cellphone + "@txt.bell.ca";
                }
                if (operatorValue == "Telus")
                {
                    employer.Cellphone = employer.Cellphone + "@msg.telus.ca";
                }

                //Email
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(employer.Email));  // replace with valid value 
                message.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                message.Subject = "Welcome to #JobForCoders!";
                message.Body = string.Format(body, "JobForCoders", "no-reply@jobforcoders.com", "Registration completed.");
                message.IsBodyHtml = true;

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

                //SMS
                var tbody = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var tmessage = new MailMessage();
                tmessage.To.Add(new MailAddress(employer.Cellphone));  // replace with valid value 
                tmessage.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                tmessage.Subject = "Welcome to #JobForCoders!";
                tmessage.Body = string.Format(tbody, "JobForCoders", "no-reply@jobforcoders.com", "Registration completed.");
                tmessage.IsBodyHtml = true;

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
                    await smtp.SendMailAsync(tmessage);
                }


                return RedirectToAction("EmployerLogin", "Home");
            }

            return View(employer);
        }

        // GET: Employers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployerID,Name,Address,City,Email,Password,Cellphone,Operator,Buzz_Words")] Employer employer, HttpPostedFileBase Logo, HttpPostedFileBase History)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employer).State = EntityState.Modified;

                if (History != null && History.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(History.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    History.SaveAs(path);

                    employer.History = fileName;
                }
                else
                {
                    db.Entry(employer).Property(m => m.History).IsModified = false;
                }

                if (Logo != null && Logo.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Logo.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    Logo.SaveAs(path);

                    employer.Logo = fileName;
                }
                else
                {
                    db.Entry(employer).Property(m => m.Logo).IsModified = false;
                }
                
                db.SaveChanges();                

                return RedirectToAction("Index", "Home");
            }
            return View(employer);
       

        }



        // GET: Employers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employer employer = db.Employers.Find(id);
            db.Employers.Remove(employer);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
