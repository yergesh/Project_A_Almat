using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectA_ConsoleCore.Models;

namespace ProjectA_ConsoleCore.DbContexes
{
    public class AppContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public AppContext()
        {
            Database.EnsureCreated();
            AddSampleData(); // Бірінші рет программаны қосқан кезде локальный база данных-қа ақпаратты жазады. Кейінгі қосу барысында комментке алып қоя салсаңыз болады. 
        }

        private void AddSampleData()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            Students.AddRange(StudentsSampleData());
            Teachers.Add(new Teacher("Малика", "Абрахманова", DateTime.Parse("05.05.1988"), "temp.teacher.oop@gmail.com", "Qwertyu123456++","malika",
                User.GetHashString("asdfg"))
            {
                MyProblems = AddProblem()
            });
            Administrators.Add(new Administrator("Бахытжан", "Ассилбеков", DateTime.Parse("02.12.1982"), "assilbeko@gmail.com","assilbekov",
                User.GetHashString("kaznitu")));
            Problems.AddRange(AddProblem());
            SaveChanges();
        }
        private List<Student> StudentsSampleData()
        {
            return new List<Student>()
            {
                new Student("1", "1", DateTime.Now, "almat_999@list.ru", 1, "1", User.GetHashString("1")),
                new Student("Алмат", "Ергеш", DateTime.Parse("12.05.2000"), "yergesh.a.k@gmail.com",3, "bigsoft", User.GetHashString("12345")),
                new Student("Асылжан", "Жансейт", DateTime.Parse("11.01.2001"),"a.e.zhanseit@gmail.com", 3, "asilzhan", User.GetHashString("qwerty")),
            };        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=project_a_db;Trusted_Connection=True;");
        }

        public List<Problem> AddProblem()
        {
            return new List<Problem>()
            {
                new Problem()
                {
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
                    },
                    Point = 400,
                    Download = DateTime.Parse("03.03.2021 06:00"),
                    Deadline = DateTime.Parse("07.03.2021 23:59")
                },
                
                new Problem()
                {
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
                    },
                    Point = 400,
                    Download = DateTime.Parse("03.03.2021 06:00"),
                    Deadline = DateTime.Parse("07.03.2021 23:59")
                },
                
                new Problem()
                {
                    Title = "Дележ яблок - 2",
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
                    },
                    Point = 300,
                    Download = DateTime.Parse("03.03.2021 06:00"),
                    Deadline = DateTime.Parse("07.03.2021 23:59")
                }
            };
        }
    }
}