using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using ProjectA_ConsoleCore.DbContexes;
using AppContext = ProjectA_ConsoleCore.DbContexes.AppContext;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public delegate void ProblemAddedHandler(ProblemAddedEventArgs arg); // делегат

        public event ProblemAddedHandler ProblemAddedNotify; //оқиға
        
        public Model()
        {
            AppContext = new AppContext();
        }

        #region Receive DataBase

        public AppContext AppContext { get; set; }
        // public List<Problem> Problems => AppContext.Problems.Include(problem => problem.TestCases).ToList();

        public List<Problem> Problems => AppContext.Teachers.SelectMany(teacher => teacher.MyProblems)
            .Include(problem => problem.TestCases).ToList();
        
        public  List<User> Users => AppContext.Students.ToList<User>().Concat(AppContext.Teachers.ToList()).ToList();
        #endregion
        
        #region Authenficated

        public bool Authenticated(string login, string passHash, out User user)
        {
            var students = AppContext.Students.Include(s => s.Attempts).ToList(); //базадан студенттер  және попыткалары туралы ақпартты қосы жүктейді.
            var teachers = AppContext.Teachers.Include(teacher => teacher.MyProblems)
                .ThenInclude(problem => problem.TestCases).ToList();
            var admins = AppContext.Administrators.ToList();

            var t1 = students.Find(u => u.Login == login && u.CheckPassword(passHash));
            if (t1 != null)
            {
                user = t1;
                return true;
            }

            var t2 = teachers.Find(u => u.Login == login && u.CheckPassword(passHash));
            if (t2 != null)
            {
                user = t2;
                return true;
            }

            var t3 = admins.Find(u => u.Login == login && u.CheckPassword(passHash));
            user = t3;
            return t3 != null;
        }

        #endregion

        #region AddResources

        public void AddTeacher(string name, string lastName, DateTime birthday, string mail, string mailPass, string login, string passwordHash)
        {
            AppContext.Teachers.Add(new Teacher(name, lastName, birthday, mail, mailPass, login, passwordHash));
            AppContext.SaveChanges();
        }

        public Attempt AddAttemption(User user, Problem problem)
        {
            var t = new Attempt(user, problem);
            user.Attempts.Add(t);
            AppContext.Update(user);
            AppContext.Update(t);
            // AppContext.SaveChanges();
            return t;
        }
        
        public bool TryAddStudent(string name, string lastName, DateTime birthday, string mail, int course, string login,
            string passwordHash)
        {
            if (AppContext.Students.Any(u => u.Login == login)) return false;
            Student student = new Student(name, lastName, birthday, mail, course, login, passwordHash);
            AppContext.Students.Add(student);
            AppContext.SaveChangesAsync();
            return true;
        }
        
        public void AddProblem(Teacher teacher, Problem problem)
        {
            teacher.MyProblems.Add(problem);
            ProblemAddedEventArgs arg = new ProblemAddedEventArgs(teacher.Name, teacher.LastName, problem.Title, problem.Point, problem.Download.ToString(CultureInfo.InvariantCulture), problem.Deadline.ToString(CultureInfo.InvariantCulture));
            ProblemAddedNotify?.Invoke(arg);
           
            AppContext.Update(teacher);
            AppContext.Update(problem);
            // AppContext.SaveChanges();
        }

        #endregion
        
        #region Methods

        public  void NotifyStudent(ProblemAddedEventArgs arg)
        {
            foreach (var user in Users.OfType<Student>())
            {
                user.Notifications.Add(arg);
            }
        }

        public bool RemoveUser(User user)
        {
            if (user == null) return false;
            if (user.Role == Role.Student)
            {
                AppContext.Students.Remove(user as Student);
            }
            else if (user.Role == Role.Teacher)
            {
                AppContext.Teachers.Remove(user as Teacher);
            }

            AppContext.SaveChanges();
            return true;
        }
        #endregion

        #region SMTP

        public void SendMessageToMail(User user)
        {
            Teacher teacher = user as Teacher;
            Student student = Users.OfType<Student>().ToList()[0];
            if (teacher != null)
            {
                SmtpClient client = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = teacher.Mail,
                        Password = teacher.MailPass
                    }
                };
                
                MailAddress fromEmail = new MailAddress(teacher.Mail);
                List<MailAddress> toEmails = new List<MailAddress>();
                foreach (var item in Users.OfType<Student>())
                    toEmails.Add(new MailAddress(item.Mail, "Everyone"));

                string text = "";
                
                for (int i = 0; i < student.Notifications.Count; i++)
                {
                    text +=
                        $"\t{i + 1}) Мұғалім: {student.Notifications[i].FromTeacherName} {student.Notifications[i].FromTeacherLastName}\n";
                    text += $"\t\t Есеп аты: {student.Notifications[i].Title}\n";
                    text += $"\t\t Балы: {student.Notifications[i].Point}\n";
                    text += $"\t\t Жүктелген уақыты: {student.Notifications[i].Download}\n";
                    text += $"\t\t Дедлайн уақыты: {student.Notifications[i].Deadline}\n";
                }

                List<MailMessage> messages = new List<MailMessage>();
                foreach (var item in Users.OfType<Student>())
                {
                    messages.Add(new MailMessage()
                    {
                        From = fromEmail,
                        Subject = $"Сізде {student.Notifications.Count} жаңа хабарлама бар",
                        Body = text
                    });
                }

                for (int i = 0; i < Users.OfType<Student>().Count(); i++)
                {
                    messages[i].To.Add(toEmails[i]);
                }

                try
                {
                    foreach (var message in messages)
                    {
                        client.Send(message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        

        #endregion
    }
}