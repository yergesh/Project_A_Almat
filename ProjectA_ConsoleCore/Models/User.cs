using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ProjectA_ConsoleCore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }

        public string Mail { get; set; }
        public string Login { get; set; }
        public Role Role { get; set; }
        
        public string PasswordHash { get; set; }
        public List<Attempt> Attempts { get; set; }

        public User(string name, string lastName, DateTime birthday, string mail, string login, string passwordHash) 
        {
            Name = name;
            LastName = lastName;
            Birthday = birthday;
            Mail = mail;
            Login = login;
            PasswordHash = passwordHash;
            Attempts = new List<Attempt>();
        }
        public User()
        {
            
        }
        
        public bool CheckPassword(string hash) => PasswordHash.Equals(hash);

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{Name} {LastName}  / {Login} /{Birthday:d}";
        }
    }
}