using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Attempt
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime ShippingTime { get; set; }
        public Problem Problem { get; set; }
        public List<TestCase> TestCases { get; set; }
        public Verdict Verdict { get; set; }

        public Attempt(User user, Problem problem)
        {
            User = user;
            ShippingTime = DateTime.Now;
            Problem = problem;
            TestCases = new List<TestCase>();
        }

        public Attempt()
        {
            
        }
    }
}