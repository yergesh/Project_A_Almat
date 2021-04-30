namespace ProjectA_ConsoleCore.Models
{
    public class TestCase
    {
        public int Id { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public TestCase(string input, string output)
        {
            Input = input;
            Output = output;
        }
    }
}