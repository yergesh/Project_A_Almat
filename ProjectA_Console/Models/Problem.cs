using System.Collections.Generic;

namespace ProjectA_Console.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TestCase> TestCases { get; set; }

        public void AddTestCase(string input, string output)
        {
            TestCases.Add(new TestCase(input, output));
        }

        public override string ToString()
        {
            return $"{Title} [id={Id}]";
        }
    }
}