using JobsForCoders.Models;
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

        [HttpPost]
        public ActionResult EmployersList(SearchEmployers search)
        {
            var searchQuery = db.JobPostings.Include(jp => jp.Employer);

            if (search.City != null)
            {
                searchQuery = searchQuery.Where(jp =>
                    search.City.Contains(jp.Employer.City));
            }

            if (search.Position != null)
            {
                searchQuery = searchQuery.Where(jp =>
                    search.Position.Contains(jp.Position));
            }

            var employerList = searchQuery.Select(jp => jp.Employer).ToList();

            return View(employerList);
        }
    }
}