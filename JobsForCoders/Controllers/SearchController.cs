using JobsForCoders.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace JobsForCoders.Controllers
{
    public class SearchController : Controller
    {
        private jobsforcodersEntities db = new jobsforcodersEntities();

        // GET: Search
        public ActionResult Employers()
        {
            return View();
        }

        public ActionResult JobSeekers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployersList(SearchEmployers search)
        {
            var searchQuery = db.JobPostings.Include(jp => jp.Employer);

            if (search.City != null)
            {
                searchQuery = searchQuery.Where(jp =>
                    jp.Employer.City.Contains(search.City));
            }

            if (search.Position != null)
            {
                searchQuery = searchQuery.Where(jp =>
                    jp.Position.Contains(search.Position));
            }

            if (search.Salary > 0)
            {
                searchQuery = searchQuery.Where(jp =>
                jp.Salary == search.Salary);
            }

            var employerList = searchQuery.Select(jp => jp.Employer).Distinct().ToList();

            return View(employerList);

        }

        [HttpPost]
        public ActionResult JobSeekersList(SearchJobSeekers search)
        {
            var searchQuery = db.JobSeekers.AsQueryable();

            if (search.City != null)
            {
                searchQuery = searchQuery.Where(js =>
                   js.City.Contains(search.City));
            }

            if (search.Address != null)
            {
                searchQuery = searchQuery.Where(js =>
                    js.Address.Contains(search.Address));
            }

            if (search.Buzz_Words != null)
            {
                searchQuery = searchQuery.Where(js =>
                    js.Buzz_Words.Contains(search.Buzz_Words));
            }

            if (search.Birthdate != DateTime.MinValue)
            {
                searchQuery = searchQuery.Where(js =>
                    js.Birthdate >= search.Birthdate);     
            }

            var jobSeekerList = searchQuery.Distinct().ToList();

            return View(jobSeekerList);

        }


    }
}