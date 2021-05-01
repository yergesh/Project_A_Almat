using System;
using System.Collections.Generic;
using System.Linq;
using ProjectA_ConsoleCore.Helper;
using ProjectA_ConsoleCore.Models;
using  static System.Console;

namespace ProjectA_ConsoleCore.Views
{
    public class View
    {
        #region Menu

        public void MainMenu()
        {
            WriteLine("[1] - Кіру\n" +
                      "[2] - Тіркелу\n" +
                      "[0] - Шығу");
        }

        public void ProfileMenu()
        {
            Clear();
            
            WriteLine("[1] - Іздеу\n" +
                      "[2] - Есептер\n" +
                      "[3] - Профиль\n" +
                      "[0] - Артқа");
        }
        
        //Мұғалім менюі
        public void TeacherMenu(string name)
        {
            Clear();
            ShowHappy(name);
            WriteLine("[1] - Іздеу\n" +
                      "[2] - Есептер\n" +
                      "[3] - Профиль\n" +
                      "[4] - Студенттер үлгерімін бақылау\n" +
                      "[0] - Артқа");
        }
        public void StudentProblemMenu()
        {
            WriteLine("[1] - Жіберу\n" +
                      "[2] - Менің жауаптарым\n" +
                      "[0] - Артқа");
        }

        public void TeacherProblemMenu()
        {
            WriteLine("[1] - Барлық есептер\n" +
                      "[2] - Менің есептерім\n" +
                      "[3] - Есеп қосу\n" +
                      "[0] - Артқа");
        }
        
        public void AdministratorMenu(string name)
        {
            Clear();
            ShowHappy(name, Role.Administrator);
            WriteLine("[1] - Мұғалім қосу\n" +
                      "[2] - Қолданушыны жою\n" +
                      "[3] - Профиль\n" +
                      "[0] - Артқа");
        }
        
        public void EditMenu()
        {
            WriteLine("[1] - Парольді өзгерту\n" +
                      "[0] - Артқа");
        }
        
        public void SendMessageToTheStudentMenu()
        {
            WriteLine("[0] - Артқа");
        }
        
        #endregion

        #region Emodji

        public void ShowError(string afterText = "")
        {
            ForegroundColor = ConsoleColor.Red;
            Write("Қате, қайтадан енгізіңіз: ");
            ForegroundColor = ConsoleColor.White;
            WriteLine(afterText);
        }
        
        public void ShowHappy(string name, Role role = Role.Student)
        {
            if (role == Role.Administrator)
                ForegroundColor = ConsoleColor.Magenta;
            else 
                ForegroundColor = ConsoleColor.Green;
            WriteLine($"Қош келдіңіз, {name}!");
            ForegroundColor = ConsoleColor.White;
        }
        
        public void GoodBye()
        {
            Clear();
            Print("Сау болыңыз!", ConsoleColor.Green);
        }
        
        #endregion

        #region Read
        
        public int ReadInt(string key = "int", int maxValue = Int32.MaxValue, ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}>> ");

            var f = ForegroundColor;
            ForegroundColor = color;
            int res=-1;
            while (!int.TryParse(ReadLine(), out res) || res > maxValue)
            {
                ShowError();
                Print($"{key}>> ");
            }

            return res;
        }
        public string ReadPass(string message = "", ConsoleColor color = ConsoleColor.White)
        {
            return User.GetHashString(ReadString(message == "" ? "Пароль: " : message, color).Trim());
        }
        public DateTime ReadDate(string key = "", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key} ");
            ConsoleColor f = ForegroundColor;
            ForegroundColor = color;
            DateTime res;
            while(!DateTime.TryParse(ReadLine(), out res))
            {
                ShowError();
                Print($"{key} ");
            }
            return res;
        }

        public string ReadString(string key = "string", ConsoleColor color = ConsoleColor.White)
        {
            Print($"{key}");
            var f = ForegroundColor;
            ForegroundColor = color;
            var res = ReadLine();
            ForegroundColor = f;
            return res;
        }

        public string ReadRichString(string key = "Есептің берілгенін ашылған терезеге жазып, сақтаңыз (Ctrl+S). Есептің берілгенін жазып болған соң ашылған терезені жабыңыз (Enter - OK)")
        {
            Print($"{key} ");
            ReadKey();
            WriteLine();
            FileHelper fh = new FileHelper();
            string text = fh.GetTextFromEditor();
            return text;
        }
        
        public List<TestCase> ReadTestCases()
        {
            List<TestCase> cases = new List<TestCase>();
            do
            {
                Clear();
                var input = ReadString("Тесттің кіріс мәндерін бір қатарға бос орын арқылы бөліп жазыңыз:\n");
                var output = ReadString("Тесттің шығыс мәндерін бір қатарға бос орын арқылы бөліп жазыңыз:\n");
                cases.Add(new TestCase(input, output));
                
            } while (YesOrNo("Тағы бір тест қосқыңыз келеді ме? "));

            return cases;
        }
        
        public bool YesOrNo(string message="")
        {
            string choice;
            do
            {
                choice = ReadString($"{message} (y/n) ").ToLower();
            } while (choice != "y" && choice != "n");

            return choice == "y";
        }

        public void Wait() => ReadKey();
        
        #endregion
        
        #region Print

        public void Print(List<Attempt> attempts)
        {
            if (attempts == null || attempts.Count == 0)
            {
                Print("Сіз ештеңе жібермегенсіз");
                Wait();
            }
            else
            {
                WriteLine(new string('-', 70));
                WriteLine($"|{"ID",5}|{"Аты", 20}|{"Уақыты", 20}|{"Вердикт", 20}|");
                WriteLine(new string('-', 70));
                foreach (var attempt in attempts)
                {
                    WriteLine($"|{attempt.Id,5}|{attempt.Problem.Title, 20}|{attempt.ShippingTime, 20}|{attempt.Verdict, 20}|");
                }
                WriteLine(new string('-', 70));
                Wait();
            }
        }
        
        public void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor c = ForegroundColor;
            ForegroundColor = color;
            Write(text);
            ForegroundColor = c;
        }

        public void Println(string text, ConsoleColor color = ConsoleColor.White) => Print(text + '\n', color);
        
        public void Print(User user)
        {
            WriteLine($"+------------------------------+");
            WriteLine($"|             Ақпарат          |");
            WriteLine($"+------------------------------+");
            WriteLine($"|Аты: {user.Name, 25}|");
            WriteLine($"|Фамилия: {user.LastName, 21}|");
            WriteLine($"|Туған күні: {user.Birthday, 18:d}|");
            WriteLine($"|Логин: {user.Login, 23}|");
            WriteLine($"+------------------------------+");
            WriteLine($"|{user.Role, 20}          |");
            WriteLine($"+------------------------------+\n");
        }

        public void Print(Problem problem)
        {
            WriteLine(new string('-',120));
            WriteLine($"{"",55}{problem.Title}");
            WriteLine(new string('-',120));
            WriteLine($"{problem.Text}");
            WriteLine(new string('-',120));
            WriteLine($"|{"Input",58}|{"Output",58}|");
            foreach (var problemTestCase in problem.TestCases)
            {
                WriteLine($"|{problemTestCase.Input.Replace('\n', ' '),58}|{problemTestCase.Output.Replace('\n', ' '),58}|");
            }
            WriteLine(new string('-',120));
        }

        //Есеп баған атаулары
        public void PrintHeaderProblem()
        {
            WriteLine($"{"No.",4}|ID|{"Тақырыбы",20}|{"Баллы",4}| {"Жүктелді",20} | {"Дедлайн",20}|");
        }
        #endregion

        #region Select

        public T Select<T>(List<T> list) where T : class
        {
            // Clear();
            // string t = new string('-', BufferWidth);
            for (int i = 0; i < list.Count; i++)
            {
                Println($"{i+1}) {list[i]}");
                // Print(t, ConsoleColor.DarkBlue);
            }
            Println("0) Артқа");
            // Print(t, ConsoleColor.DarkBlue);
            
            int index = ReadInt("Таңдаңыз: ", list.Count);
            if (index == 0) return default;
            return list[index-1];
        }
        
        #endregion

        // Кесте
        public void MonitoringStudents(List<Student> students, List<Problem>problems)
        {
            Write($"|{"ID",5}|{"Аты", 20}|{"=", 5}|{"Штраф", 7}|");
            foreach (var problem in problems)
            {
                Write($"{problem.Title, 15}|");
            }
            WriteLine($"{"Ұпай", 6}|{"Балл", 6}|");

            foreach (var student in students)
            {
                int point = student.Attempts.Where(a=>a.Verdict == Verdict.Accepted).Select(a=>a.Problem).Distinct().ToList().Count; //барлық есеп үшін уникальный барлық дұрыс жауап саны.
                int allPoint = student.Attempts.Where(a=>a.Verdict == Verdict.Accepted).Select(a=>a.Problem).ToList().Count; // әрбір есеп үшін барлық дұрыс жауап саны
                int shtraf = (student.Attempts.Count - allPoint) * 50; //штраф поинт есептелуі
                int mark = student.CurrentPoint; // жинаған ұпайы
                
                Write($"|{student.Id,5}|{student.Name, 20}|{point, 5}|{shtraf, 7}|");
                foreach (var problem in problems)
                {
                    var atts = student.Attempts.Where(a => a.Problem == problem).ToList(); // Текущий есептің барлық попыткалары
                    int cnt = atts.Count(a => a.Verdict != Verdict.Accepted); // дұрыс емес жауаптар саны
                    int acceptedCnt = atts.Count - cnt; //дұрыс жауаптар саны
                    if (acceptedCnt != 0) //дұрыс шыққан болса
                    {
                        Print($"{$"+{(atts.Count - acceptedCnt == 0?"": (atts.Count - acceptedCnt).ToString())}", 15}|", ConsoleColor.Green); //бірінші попыткадан (+) бірнеше попыткадан кейін (+n)
                    }
                    else if(cnt != 0) // шықпай қалған есеп үшін
                    {
                        Print($"{$"-{cnt}", 15}|", ConsoleColor.Red);
                    }
                    else
                    {
                        Print($"{"", 15}|");
                    }
                }
                WriteLine($"{mark, 6}|{student.Gpa, 6:F}|");
            }
        }
        //Студент хабарламасының консольдық бейнесі
        public void ShowNotifications(User user)
        {
            Student student = user as Student;
            WriteLine($"Сізде {student.Notifications.Count} жаңа хабарлама бар: ");
            for (int i = 0; i < student.Notifications.Count; i++)
            {
                WriteLine($"\t{i+1}) Мұғалім: {student.Notifications[i].FromTeacherName} {student.Notifications[i].FromTeacherLastName}");
                WriteLine($"\t\t Есеп аты: {student.Notifications[i].Title}");
                WriteLine($"\t\t Балы: {student.Notifications[i].Point}");
                WriteLine($"\t\t Жүктелген уақыты: {student.Notifications[i].Download}");
                WriteLine($"\t\t Дедлайн уақыты: {student.Notifications[i].Deadline}");
            }
            Wait();
            student.Notifications.Clear(); // Хабарламалар тізімі көрсетіліп болғаннан кейін тазаланады 
        }
    }
}