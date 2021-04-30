using System;
using System.Data.Common;
using System.Security.Policy;

namespace ProjectA_Console.Models
{
    public class Student : User
    {
        public Student(int id, string name, string lastName, DateTime birthday,
            int course, string login, int passwordHash) : base(id, name, lastName, birthday, login, passwordHash)
        {
            Course = course;
        }

        public int Course { get; set; }

        public Student()
        {
            
        }
    }
}

