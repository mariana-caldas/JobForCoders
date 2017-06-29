using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobsForCoders.Models
{
    public class SearchJobSeekers
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Buzz_Words { get; set; }
        public DateTime Birthdate { get; set; }
    }
}