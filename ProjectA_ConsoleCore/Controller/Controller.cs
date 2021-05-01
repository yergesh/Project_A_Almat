using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProjectA_ConsoleCore.Models;
using ProjectA_ConsoleCore.Views;
using static System.Console;

namespace ProjectA_ConsoleCore.Controller
{
    public class Controller
    {
        View view = new View();
        Model model = new Model();
        public User CurrentUser;

        public Controller()
        {
            model.ProblemAddedNotify += model.NotifyStudent; // ProblemAddedNotify оқиғасы үшін өңдеуші қосамыз
        }

        #region Main

        public void Main()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.MainMenu();
                cmd = view.ReadInt();
                switch (cmd)
                {
                    case 1:
                        Clear();
                        view.Print("Жүйеге кіру\n", ConsoleColor.Green);
                        if (Authenfication())
                        {
                            Clear();
                            // view.ShowHappy(CurrentUser.Name);
                            switch (CurrentUser.Role)
                            {
                                case Role.Administrator:
                                    AdministratorCommand();
                                    break;
                                case Role.Student:
                                    StudentCommand();
                                    break;
                                case Role.Teacher:
                                    TeacherCommand();
                                    break;
                                default: throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            view.ShowError();
                            ReadKey();
                        }

                        break;
                    case 2:
                        Register();
                        break;
                    case 0:
                        view.GoodBye();
                        return;
                }
            }
        }

        #endregion

        #region Authenfication && Register

        public bool Authenfication()
        {
            string name = view.ReadString("Логин: ");
            string passHash = view.ReadPass();
            return model.Authenticated(name, passHash, out CurrentUser);
        }

        public void Register()
        {
            Clear();
            view.Print("Жүйеге тіркелу\n", ConsoleColor.Green);
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            string mail = view.ReadString("Почта: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            string passwordHash = view.ReadPass();
            if (!model.TryAddStudent(name, lastName, birthday, mail, course, login, passwordHash))
                view.Print("Жүйеде бұл қолданушы бар!!!\n", ConsoleColor.Yellow);
            else view.Print("Тіркелу сәтті аяқталды!!!\n", ConsoleColor.Green);
            ReadKey();
        }

        #endregion

        #region Administrator

        public void AdministratorCommand()
        {
            int cmd;
            while (true)
            {
                view.AdministratorMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt();
                switch (cmd)
                {
                    case 1:
                        AddTeacher();
                        break;
                    case 2:
                        RemoveUser();
                        break;
                    case 3:
                        ProfileCommand();
                        break;
                    case 0: return;
                }
            }
        }

        private void AddTeacher()
        {
            Clear();
            WriteLine("Мұғалімді тіркеу");
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            string mail = view.ReadString("Почта: ");
            string mailPass = view.ReadString("Почта құпия сөзі: ");
            string login = view.ReadString("Логин: ");
            string passwordHash = view.ReadPass();
            model.AddTeacher(name, lastName, birthday, mail, mailPass, login, passwordHash);
            view.Print("Тіркелу сәтті аяқталды!!!", ConsoleColor.Green);
            ReadKey();
        }

        private void RemoveUser()
        {
            var user = view.Select(model.Users);
            if (user == null ||
                !view.YesOrNo($"{user.Name} {user.LastName} пайдаланушы аккаунты жүйеден жойылады. Сенімдісіз бе?"))
                return;
            if (model.RemoveUser(user))
            {
                view.Println("Пайдаланушы сәтті жойылды! ");
            }
            else view.Println("Пайдаланушыны жою барысында қателік шықты! ");

            view.Wait();
        }

        #endregion

        #region Student

        private void StudentCommand()
        {
            int cmd;
            view.ShowHappy(CurrentUser.Name + " " + CurrentUser.LastName);
            view.ShowNotifications(CurrentUser);
            while (true)
            {
                view.ProfileMenu();
                cmd = view.ReadInt();
                switch (cmd)
                {
                    case 1:
                        try
                        {
                            Search();
                        }
                        catch (NotImplementedException notImp)
                        {
                            view.Print("Бұл меню жасалу үстінде!!!\n", ConsoleColor.Green);
                        }

                        break;
                    case 2:
                        SelectProblems();
                        break;
                    case 3:
                        ProfileCommand();
                        break;
                    case 0: return;
                }
            }
        }

        private void StudentProblemCommand(Problem problem)
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(problem);
                view.StudentProblemMenu();
                cmd = view.ReadInt();
                switch (cmd)
                {
                    case 1:
                        Submit(problem);
                        break;
                    case 2:
                        view.Print(CurrentUser.Attempts);
                        break;
                    case 0: return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
                }
            }
        }

        #endregion

        #region Teacher

        private void TeacherCommand()
        {
            int cmd;
            while (true)
            {
                view.TeacherMenu(CurrentUser.Name + " " + CurrentUser.LastName);
                cmd = view.ReadInt(maxValue: 4);
                switch (cmd)
                {
                    case 1:
                        try
                        {
                            Search();
                        }
                        catch (NotImplementedException notImp)
                        {
                            view.Print("Бұл меню жасалу үстінде!!!", ConsoleColor.Green);
                        }

                        break;
                    case 2:
                        TeacherProblemsMenu();
                        break;
                    case 3:
                        ProfileCommand();
                        break;
                    // Мониторинг жүргізуге арналган метод
                    case 4:
                        MonitoringStudentProgress();
                        break;
                    case 0: return;
                }
            }
        }

        private void TeacherProblemsMenu()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.TeacherProblemMenu();
                cmd = view.ReadInt(maxValue: 3);
                switch (cmd)
                {
                    case 1:
                        SelectProblems();
                        break;
                    case 2:
                        SelectProblems((CurrentUser as Teacher)?.MyProblems);
                        break;
                    case 3:
                        AddProblem();
                        model.SendMessageToMail(CurrentUser);
                        break;
                    case 0: return;
                }
            }
        }

        private void AddProblem()
        {
            Clear();
            string title = view.ReadString("Есептің тақырыбы: ");
            string text = view.ReadRichString();
            List<TestCase> cases = view.ReadTestCases();
            int point = view.ReadInt("Есеп ұпайы: ");
            DateTime download = DateTime.Now;
            DateTime deadline = view.ReadDate("Дедлайн: ");
            var problem = new Problem()
            {
                Title = title,
                Text = text,
                TestCases = cases,
                Point = point,
                Download = download,
                Deadline = deadline
            };
            if (CurrentUser is Teacher teacher) model.AddProblem(teacher, problem);
            // model.AppContext.Problems.Add(problem);
            // model.AppContext.Update(CurrentUser);
            // model.AppContext.Update(problem);
        }

        private void MonitoringStudentProgress()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.MonitoringStudents(model.Users.OfType<Student>().ToList(), model.Problems); // Мониторинг кестесі
                view.SendMessageToTheStudentMenu();
                cmd = view.ReadInt(maxValue: 2);
                switch (cmd)
                {
                    case 0: return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
                }
            }
        }

        #endregion

        #region AllUserCommand

        private void ProfileCommand()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(CurrentUser);
                view.EditMenu();
                cmd = view.ReadInt();
                switch (cmd)
                {
                    case 1:
                        EditPass();
                        break;
                    case 0: return;
                }
            }
        }

        private void EditPass()
        {
            string lastPass = view.ReadPass("Ескі парольіңізді енгізіңіз: ");
            if (lastPass != CurrentUser.PasswordHash)
            {
                view.Println("Пароль қате! Парольді өзгерте алмайсыз");
                return;
            }

            CurrentUser.PasswordHash = view.ReadPass("Жаңа пароль енгізіңіз: ");
            model.AppContext.Update(CurrentUser);
            model.AppContext.SaveChanges();
            view.Print("Пароль сәтті түрде өзгертілді!!!\n", ConsoleColor.Green);
            view.Wait();
        }

        private void SelectProblems(List<Problem> list = null)
        {
            Clear();
            view.PrintHeaderProblem();
            if (list == null)
            {
                list = model.Problems;
            }

            var problem = view.Select(list);
            if (problem == null) return;
            StudentProblemCommand(problem);
        }

        private void Search()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Compiler

        private string GenerateRuntimeConfig()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() {Indented = true}))
                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("runtimeOptions");
                    writer.WriteStartObject("framework");
                    writer.WriteString("name", "Microsoft.NETCore.App");
                    writer.WriteString("version", RuntimeInformation.FrameworkDescription.Replace(".NET Core ", ""));
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public void Submit(Problem problem)
        {
            string sourcePath =
                view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе тышқанмен сүйреп әкеліңіз\n",
                    ConsoleColor.Green);
            sourcePath = sourcePath.Trim(new[] {'\'', '\"'});
            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(File.ReadAllText(sourcePath)));
            var assemblyPath = "test.exe";
            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var compilation = CSharpCompilation.Create(Path.GetFileName(assemblyPath))
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication)).AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll")))
                .AddSyntaxTrees(syntaxTree);
            var result = compilation.Emit(assemblyPath);
            File.WriteAllText(Path.ChangeExtension(assemblyPath, "runtimeconfig.json"), GenerateRuntimeConfig());
            Attempt attempt = model.AddAttemption(CurrentUser, problem);
            if (!result.Success)
            {
                view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
            }
            else
            {
                RunSolution(problem, assemblyPath, ref attempt);
            }

            model.AppContext.SaveChanges(); // әрбір жіберілген попыткаларды базада сақтау
            // TODO: model.Attempts.Add(attempt);
        }

        private void RunSolution(Problem problem, string assembly, ref Attempt attempt)
        {
            Verdict verdict = Verdict.Wrong_answer;
            foreach (var testCase in problem.TestCases)
            {
                Process process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = assembly;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.ErrorDialog = false;
                process.Start();
                StreamWriter stdInputWriter = process.StandardInput;
                StreamReader stdOutputReader = process.StandardOutput;
                stdInputWriter.WriteLine(testCase.Input.Replace(' ', '\n'));
                string res = stdOutputReader.ReadToEnd();
                if (string.Compare(res.Trim(), testCase.Output, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    verdict = Verdict.Accepted;
                }
                else
                {
                    verdict = Verdict.Wrong_answer;
                    break;
                }

                var result = new AttemptionResult(testCase.Input, testCase.Output, res);
                attempt.TestCases.Add(result);
                // process.Kill();
            }

            //Негізгі есептеулер
            if (attempt.User is Student student) // студент жағдайын ғана қарастыру
            {
                attempt.Verdict = verdict;
                int acceptedCnt = attempt.User.Attempts.Where(a => a.Problem == problem)
                    .Count(a => a.Verdict == Verdict.Accepted); // Текущий есептің дұрыс жауаптар саны
                int cnt = attempt.User.Attempts.Where(a => a.Problem == problem).ToList().Count -
                          acceptedCnt; // штраф анықтау үшін текущий есептің барлық попыткасынан дұрыс жауап санын алып тастау керек. 
                if (verdict == Verdict.Accepted && acceptedCnt == 1
                ) // тек бірінші дұрыс қана қабылданады. екінші рет қайта жібергенде, ұпай қосылмайды.
                    student.CurrentPoint +=
                        (problem.Point - (cnt * 50) >= 150
                            ? problem.Point - (cnt * 50)
                            : 150
                        ); // штраф ұпайын алып тастап currentPoint - қа қосады. Егер өте көп попытка жіберіп, алған ұпайы минимальды көрсеткіштен төмен болса, оғпн минимум ұпай беріледі.
                double percent = 0;
                if (model.Problems != null)
                {
                    percent = student.CurrentPoint * 1.0 / model.Problems.Sum(s => s.Point) * 100; // үлгерімін есептеу
                }

                student.Gpa = percent;
            }

            if (verdict == Verdict.Accepted)
            {
                view.Print("Дұрыс жауап!\n", ConsoleColor.Green);
            }
            else view.Print("Қате жауап!\n", ConsoleColor.Red);

            ReadKey();
        }

        #endregion
    }
}