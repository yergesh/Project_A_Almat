using System;

namespace ProjectA_Console.Models
{
    public class Administrator : User
    {

        public Administrator(int id, string name, string lastName, DateTime birthday, string login, int passwordHash) : base(id, name, lastName,
            birthday, login, passwordHash)
        {
            
        }
    }
}