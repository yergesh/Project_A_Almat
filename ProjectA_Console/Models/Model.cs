using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectA_Console.Models
{
    public class Model
    {
        public List<User> Users { get; set; }
        public List<Problem> Problems { get; set; }
        public List<Attempt> Attempts { get; set; }

        public Model()
        {
            // Students = new List<Student>();
            // Problems = new List<Problem>();
            Attempts = new List<Attempt>();

            Problems = LoadExampleProblems();
            Users = LoadExampleStudents();
        }

        private List<Problem> LoadExampleProblems()
        {
            return new List<Problem>()
            {
                new Problem()
                {
                    Id = 1,
                    Title = "a*2",
                    Text =
                        @"Бір бүтін сан беріледі. осы санды екіге көбейткендегі мәнін экранға шығару керек",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("4", "8"),
                        new TestCase("8", "16")
                    }
                },
                new Problem()
                {
                    Id = 2,
                    Title = "Гипотенуза",
                    Text =
                        @"Дано два числа a и b. Найдите гипотенузу треугольника с заданными катетами.

Входные данные
В двух строках вводятся два числа (числа целые,положительные, не превышают 1000).

Выходные данные
Выведите ответ на задачу.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n4", "5"),
                        new TestCase("8\n6", "10")
                    }
                },
                new Problem()
                {
                    Id = 3,
                    Title = "Дележ яблок - 1",
                    Text =
                        @"N школьников делят K яблок поровну, неделящийся остаток остается в корзинке. Сколько яблок достанется каждому школьнику?

Входные данные
Программа получает на вход числа N и K.

Выходные данные
Программа должна вывести искомое количество яблок.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n14", "4"),
                        new TestCase("8\n15", "1"),
                        new TestCase("8\n17", "2")
                    }
                },
                new Problem()
                {
                    Id = 4,
                    Title = "Дележ яблок - 1",
                    Text =
                        @"N школьников делят K яблок поровну, неделящийся остаток остается в корзинке. Сколько яблок останется в корзинке?

Входные данные
Программа получает на вход числа N и K.

Выходные данные
Программа должна вывести искомое количество яблок.",
                    TestCases = new List<TestCase>()
                    {
                        new TestCase("3\n14", "2"),
                        new TestCase("8\n15", "7"),
                        new TestCase("8\n17", "1")
                    }
                }
            };
        }

        private List<User> LoadExampleStudents()
        {
            return new List<User>()
            {
                new Student(1, "Асылжан", "Жансейт", DateTime.Parse("11.01.2001"), 3, "asilzhan",
                    "qwerty".GetHashCode()),
                new Student(1, "Алмат", "Ергеш", DateTime.Parse("12.05.2000"), 3, "almat", "1234".GetHashCode()),
                new Teacher(2,"Малика", "Абдрахманова", DateTime.Now, "malika", "oopsila".GetHashCode())
            };
        }

        public List<Problem> GetProblemsByTeacherId(int teacherId)
        {
            return Problems.FindAll(problem => problem.Id == teacherId);
        }

        public bool TryAddStudent(string name, string lastName, DateTime born, int course, string login, int passHash)
        {
            if (Users.Exists(s => (s.Name == name && s.LastName == lastName) || s.Login == login))
            {
                return false; // Бұндай атты немесе логинді студент бар болса
            }

            int id = 1;
            if (Users.Count != 0) id = Users.Max(s => s.Id) + 1;
            Student student = new Student(id, name, lastName, born, course, login, passHash);
            Users.Add(student);
            return true;
        }

        public Problem AddProblem(string title, string text)
        {
            int id = 1;
            if (Problems.Count != 0) id = Problems.Max(s => s.Id) + 1;
            Problem problem = new Problem() {Id = id, Title = title, Text = text};
            return problem;
        }

        public Attempt AddAttemption(User user, Problem problem)
        {
            int id = 1;
            if (Attempts.Count != 0) id = Attempts.Max(a => a.Id) + 1;
            var attempt = new Attempt(id, user, problem);
            return attempt;
        }

        public bool Authenticated(string login, int password, out User user)
        {
            user = Users.Find(std => std.Login == login && std.CheckPassword(password));
            return !(user is null);
        }

        public List<Attempt> GetAttemptsOfStudent(Problem problem, Student currentStudent)
        {
            return Attempts.FindAll(attempt => attempt.Problem == problem && attempt.User == currentStudent);
        }
    }
}