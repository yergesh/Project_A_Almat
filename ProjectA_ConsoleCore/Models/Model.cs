using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using AppContext = ProjectA_ConsoleCore.DbContexes.AppContext;

namespace ProjectA_ConsoleCore.Models
{
    public class Model
    {
        public delegate void ProblemAddedHandler(ProblemAddedEventArgs arg); // делегат

        public event ProblemAddedHandler ProblemAddedNotify; //оқиғаның анықталуы
        
        public Model()
        {
            AppContext = new AppContext();
        }

        #region Receive DataBase

        public AppContext AppContext { get; set; }
        // public List<Problem> Problems => AppContext.Problems.Include(problem => problem.TestCases).ToList();

        public List<Problem> Problems => AppContext.Teachers.SelectMany(teacher => teacher.MyProblems)
            .Include(problem => problem.TestCases).ToList();
        
        public List<User> Users => AppContext.Students.ToList() // Студенттерді қосамыз
            .Concat<User>(AppContext.Teachers.ToList())         // Оқытушыларды қосамыз
            .Concat(AppContext.Administrators.ToList()).ToList(); // Администраторларды қосамыз
        #endregion
        
        #region Authenficated

        public bool Authenticated(string login, string passHash, out User user)
        {
            /*----------Аутентификация логикасы жақсартылды----------*/
            return (user = Users.Find(u => u.Login == login && u.CheckPassword(passHash))) != null;
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
    /*----------Есеп қосу логикасы жақсартылды----------*/
    
    teacher.MyProblems.Add(problem); // current teacher есептер базасына қосу
    
    ProblemAddedEventArgs arg = new ProblemAddedEventArgs(teacher.Name, teacher.LastName, problem.Title,
        problem.Point, problem.Download.ToString(CultureInfo.InvariantCulture),
        problem.Deadline.ToString(CultureInfo.InvariantCulture)); //Хабарлау оқиғасы үшін өңдеуші қосыңыз
    ProblemAddedNotify?.Invoke(arg); // оқиғаны шақырмас бұрын оны null-ға тексеріп барып шақырамыз.
   
    AppContext.Update(teacher); //User->teacher базасын жаңартамыз.
    AppContext.Update(problem); // Есептер базасын жаңартамыз.
    // AppContext.SaveChanges();
}
#endregion

#region Methods

/*----------Әрбір студентке мұғалім қаншалықты есеп қосқандығы туралы хабарламалар жиынтығын әзірлеу методы.----------*/
public  void NotifyStudent(ProblemAddedEventArgs arg)
{
    foreach (var user in Users.OfType<Student>())
    {
        user.Notifications.Add(arg); // Пайда болған оқиғаны әрбір студенттің оқиғалар тізіміне енгізу.
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

#region Send message to email students

        /*----------Барлық студентке smtp протоколы арқылы электрондық почтасына пайда болған хабарламалар тізімін жіберу----------*/
        public void SendMessageToMail(User user) // параметрі user, яғни current teacher қабылдайды
        {
            var teacher = user as Teacher; // downcast
            var student = Users.OfType<Student>().ToList()[^1]; //барлық студент хабарламалары біркелкі болғандықтан, бізге қай студент хабарламасы екендігі маңызды емес. //[^1] LastStudentData
            if (teacher == null) return;
            
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                // Почта кұпия кілті негізіндегі аутентификациядан өту
                Credentials = new NetworkCredential()
                {
                    UserName = teacher.Mail, // current teacher почтасы 
                    Password = teacher.MailPass // Current teacher құпия кілті
                }
            };
                
            MailAddress fromEmail = new MailAddress(teacher.Mail); // кімнен
            List<MailAddress> toEmails = new List<MailAddress>(); // кімдерге
           
            //барлық студент почтасын қабылдаушылар тізіміне қосамыз
            foreach (var item in Users.OfType<Student>())
                toEmails.Add(new MailAddress(item.Mail, "Everyone"));

            string text = ""; // қандай хабарлама барады?
                
            //Бізде пайда болған оқиғалар тізімін строкага түрлендіреміз
            for (int i = 0; i < student.Notifications.Count; i++)
            {
                text +=
                    $"{i + 1}) Мұғалім: {student.Notifications[i].FromTeacherName} {student.Notifications[i].FromTeacherLastName}\n";
                text += $"\tЕсеп аты: {student.Notifications[i].Title}\n";
                text += $"\tБалы: {student.Notifications[i].Point}\n";
                text += $"\tЖүктелген уақыты: {student.Notifications[i].Download}\n";
                text += $"\tДедлайн уақыты: {student.Notifications[i].Deadline}\n";
            }

            List<MailMessage> messages = new List<MailMessage>(); 
            // әрбір студенттің хат формасын жасаймыз.
            foreach (var item in Users.OfType<Student>())
            {
                messages.Add(new MailMessage()
                {
                    From = fromEmail, // кімнен екендігі
                    Subject = $"Сізде {student.Notifications.Count} жаңа хабарлама бар", // тақырыбы
                    Body = text // мазмұны
                });
            }

            //сәйкесінше әрбір хатқа студент почтасын тіркейміз. 
            for (int i = 0; i < Users.OfType<Student>().Count(); i++)
            {
                messages[i].To.Add(toEmails[i]);
            }

            //исключениеге тексереміз
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
        #endregion
    }
}
