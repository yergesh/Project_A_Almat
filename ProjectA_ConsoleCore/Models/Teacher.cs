using System;
using System.Collections.Generic;

namespace ProjectA_ConsoleCore.Models
{
    public class Teacher : User
    {
        public List<Problem> MyProblems { get; set; }
        public string MailPass { get; set; }
        public Teacher(string name, string lastName, DateTime birthday, string mail, string mailPass, string login, string passwordHash) : base(
            name, lastName,
            birthday, mail, login, passwordHash)
        {
            MailPass = mailPass;
            Role = Role.Teacher;
            MyProblems = new List<Problem>();
        }
    }
}