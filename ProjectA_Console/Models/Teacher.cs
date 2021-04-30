using System;

namespace ProjectA_Console.Models
{
    public class Teacher : User
    {
        public Teacher(int id, string name, string lastName, DateTime birthday, string login, int passwordHash) : base(
            id, name, lastName,
            birthday, login, passwordHash)
        {
        }
    }
}