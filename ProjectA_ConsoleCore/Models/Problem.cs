using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TestCase> TestCases { get; set; }

        public DateTime Download { get; set; } // Есеп системаға жүктелген уақыты қосылды
        public DateTime Deadline { get; set; } // Дедлайн уақыты қосылды

        public int Point { get; set; } // Есептің ұпайы

        public void AddTestCase(string input, string output)
        {
            TestCases.Add(new TestCase(input, output));
        }

        //Асыра жүктеу әдісі түрленді
        public override string ToString()
        {
            return ($" |{Id} |{Title,20}| {Point,4}| {Download,20} | {Deadline,20}|").PadLeft(5);
        }
    }
}