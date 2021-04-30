namespace ProjectA_Console.Models
{
    public class TestCase
    {
        public string Input { get; set; }
        public string Output { get; set; }

        public TestCase(string input, string output)
        {
            Input = input;
            Output = output;
        }
    }
}