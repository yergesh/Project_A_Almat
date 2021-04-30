using System;

namespace ProjectA_Console.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Login { get; set; }

        private readonly int _passwordHash;
        

        public User(int id, string name, string lastName, DateTime birthday, string login, int passwordHash)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Birthday = birthday;
            Login = login;
            _passwordHash = passwordHash;
        }

        public User()
        {
            
        }
        
        public bool CheckPassword(int hash) => _passwordHash.Equals(hash);
    }
}