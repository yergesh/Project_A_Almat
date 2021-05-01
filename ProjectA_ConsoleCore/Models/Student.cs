using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Student : User
    {
        public Student(string name, string lastName, DateTime birthday, string mail,
            int course, string login, string passwordHash) : base(name, lastName, birthday, mail, login, passwordHash)
        {
            Course = course;
            Role = Role.Student;
        }
        public int Course { get; set; }
        
        public int CurrentPoint { get; set; }
        public double Gpa { get; set; }
        
        public List<ProblemAddedEventArgs> Notifications { get; set; } = new List<ProblemAddedEventArgs>();
    }
}

