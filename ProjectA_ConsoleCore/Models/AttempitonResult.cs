namespace ProjectA_ConsoleCore.Models
{
    public class AttemptionResult : TestCase
    {
        public AttemptionResult(string input, string expected, string received) : base(input, expected)
        {
            Received = received; // инициализация жасалды
        }

        public string Received { get; set; }
    }
}