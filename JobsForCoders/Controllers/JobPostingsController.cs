using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JobsForCoders.Models;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JobsForCoders.Controllers
{
    public class JobPostingsController : Controller
    {
        private jobsforcodersEntities db = new jobsforcodersEntities();

        // GET: JobPostings
        public ActionResult Index()
        {
            var jobPostings = db.JobPostings.Include(j => j.Employer);
            if (Session["LoggedUserID"] != null)
            {
                var employeerId = (int)Session["LoggedUserID"];
                jobPostings = jobPostings.Where(jp => jp.EmployerID == employeerId);
            }
            return View(jobPostings.ToList());

        }

        // GET: JobPostings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobPosting jobPosting = db.JobPostings.Find(id);
            if (jobPosting == null)
            {
                return HttpNotFound();
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Create
        public ActionResult Create()
        {
            ViewBag.EmployerID = new SelectList(db.Employers, "EmployerID", "Name");
            return View();
        }

        // POST: JobPostings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "JobID,EmployerID,Name,Position,Description,Buzz_Words,Filled")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                db.JobPostings.Add(jobPosting);
                db.SaveChanges();

                //This will load all JobSeekers where the Buzz_Words contains jobposting Buzz_Words
                var buzzWords = jobPosting.Buzz_Words.Split(',').ToList();
                var jobSeekerList = db.JobSeekers.Where(js => js.Buzz_Words.Contains(jobPosting.Buzz_Words));

                foreach (var jobSeeker in jobSeekerList)
                {
                    //Determine the Cellphone Operator
                    var operatorValue = jobSeeker.Operator;
                    if (operatorValue == "Virgin")
                    {
                        jobSeeker.Cellphone = jobSeeker.Cellphone + "@vmobile.ca";
                    }
                    if (operatorValue == "Fido" || operatorValue == "Microcell")
                    {
                        jobSeeker.Cellphone = jobSeeker.Cellphone + "@fido.ca";
                    }
                    if (operatorValue == "Rogers")
                    {
                        jobSeeker.Cellphone = jobSeeker.Cellphone + "@fido.ca";
                    }
                    if (operatorValue == "Bell")
                    {
                        jobSeeker.Cellphone = jobSeeker.Cellphone + "@txt.bell.ca";
                    }
                    if (operatorValue == "Telus")
                    {
                        jobSeeker.Cellphone = jobSeeker.Cellphone + "@msg.telus.ca";
                    }

                    //Email
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(jobSeeker.Email));  // replace with valid value 
                    message.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                    message.Subject = "News from #JobForCoders!";
                    message.Body = string.Format(body, "JobForCoders", "no-reply@jobforcoders.com", "A new position that matches your buzz words have just been registered.");
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
                    tmessage.To.Add(new MailAddress(jobSeeker.Cellphone));  // replace with valid value 
                    tmessage.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                    tmessage.Subject = "News from #JobForCoders!";
                    tmessage.Body = string.Format(tbody, "JobForCoders", "no-reply@jobforcoders.com", "A new position that matches your buzz words have just been registered.");
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
                }


                return RedirectToAction("Index");
            }

            ViewBag.EmployerID = new SelectList(db.Employers, "EmployerID", "Name", jobPosting.EmployerID);
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobPosting jobPosting = db.JobPostings.Find(id);
            if (jobPosting == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployerID = new SelectList(db.Employers, "EmployerID", "Name", jobPosting.EmployerID);
            return View(jobPosting);
        }

        // POST: JobPostings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "JobID,EmployerID,Name,Position,Description,Buzz_Words,Filled")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobPosting).State = EntityState.Modified;
                db.SaveChanges();



                if (jobPosting.Filled == true)
                {

                    var buzzWords = jobPosting.Buzz_Words.Split(',').ToList();
                    var jobSeekerList = db.JobSeekers.Where(js => js.Buzz_Words.Contains(jobPosting.Buzz_Words));

                    foreach (var jobSeeker in jobSeekerList)
                    {
                        //Determine the Cellphone Operator
                        var operatorValue = jobSeeker.Operator;
                        if (operatorValue == "Virgin")
                        {
                            jobSeeker.Cellphone = jobSeeker.Cellphone + "@vmobile.ca";
                        }
                        if (operatorValue == "Fido" || operatorValue == "Microcell")
                        {
                            jobSeeker.Cellphone = jobSeeker.Cellphone + "@fido.ca";
                        }
                        if (operatorValue == "Rogers")
                        {
                            jobSeeker.Cellphone = jobSeeker.Cellphone + "@fido.ca";
                        }
                        if (operatorValue == "Bell")
                        {
                            jobSeeker.Cellphone = jobSeeker.Cellphone + "@txt.bell.ca";
                        }
                        if (operatorValue == "Telus")
                        {
                            jobSeeker.Cellphone = jobSeeker.Cellphone + "@msg.telus.ca";
                        }

                        //Email
                        var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                        var message = new MailMessage();
                        message.To.Add(new MailAddress(jobSeeker.Email));  // replace with valid value 
                        message.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                        message.Subject = "News from #JobForCoders!";
                        message.Body = string.Format(body, "JobForCoders", "no-reply@jobforcoders.com", "A new position that matches your buzz words have been just filled.");
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
                        tmessage.To.Add(new MailAddress(jobSeeker.Cellphone));  // replace with valid value 
                        tmessage.From = new MailAddress("jobforcoders@hotmail.com");  // replace with valid value
                        tmessage.Subject = "News from #JobForCoders!";
                        tmessage.Body = string.Format(tbody, "JobForCoders", "no-reply@jobforcoders.com", "A new position that matches your buzz words have been just filled.");
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
                    }


                }

                return RedirectToAction("Index");
            }
            ViewBag.EmployerID = new SelectList(db.Employers, "EmployerID", "Name", jobPosting.EmployerID);
            return View(jobPosting);
        }

        // GET: JobPostings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobPosting jobPosting = db.JobPostings.Find(id);
            if (jobPosting == null)
            {
                return HttpNotFound();
            }
            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobPosting jobPosting = db.JobPostings.Find(id);
            db.JobPostings.Remove(jobPosting);
            db.SaveChanges();
            return RedirectToAction("Index");
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
