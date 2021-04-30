using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;
using ProjectA_Console.Models;
using ProjectA_Console.Views;
using static System.Console;
namespace ProjectA_Console.Controller
{
    public class Controller
    {
        View view = new View();
        Model model = new Model();
        public User CurrentUser;
        public Student CurrentStudent;

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
                             view.ShowHappy(CurrentUser.Name);
                             if(CurrentUser is Student)
                                 StudentCommand();
                             else if (CurrentUser is Teacher)
                                 TeacherCommand();
                             else
                                 AdministratorCommand();
                        } 
                        else
                            view.ShowError();
                        ReadKey();
                        break;
                    case 2:
                        Clear();
                        view.Print("Жүйеге тіркелу\n", ConsoleColor.Green);
                        Register(); break;
                    case 0:
                        return;
                }
            }
        }
        
        public bool Authenfication()
        {
            string name = view.ReadString("Логин: ");
            int password = view.ReadPass();
            return model.Authenticated(name, password, out CurrentUser);
        }

        public void Register()
        {
            WriteLine("Тіркелу");
            string name = view.ReadString("Аты: ");
            string lastName = view.ReadString("Тегі: ");
            DateTime birthday = view.ReadDate("Туған күні [dd:MM:yyyy]: ");
            int course = view.ReadInt("Курс: ");
            string login = view.ReadString("Логин: ");
            int passwordHash = view.ReadInt("Пароль: ");
            if (!model.TryAddStudent(name, lastName, birthday, course, login, passwordHash))
                view.Print("Жүйеде бұл қолданушы бар!!!", ConsoleColor.Yellow);
            else
                view.Print("Тіркелу сәтті аяқталды!!!", ConsoleColor.Green);
            ReadKey();
        }
        
        private void AdministratorCommand()
        {
            
        }

        public void StudentCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        Search();
                        break;
                    case 2:
                        ShowProblems();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void Search()
        {
            throw new NotImplementedException();
        }

        public void TeacherCommand()
        {
            int cmd;
            while (true)
            {
                view.ProfileMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        ShowProblems();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void ShowProblems()
        {
            Clear();
            view.Print(model.Problems);
            Problem p = model.Problems[view.ReadInt(maxValue: model.Problems.Count)];

            ProblemMenu(p);
        }

        private void ProblemMenu(Problem problem)
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
                        view.Print(model.GetAttemptsOfStudent(problem, CurrentStudent));
                        break;
                    case 0:
                        return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
                }
                
            }
        }

        public void Submit(Problem problem)
        {
            string sourcePath = view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе тышқанмен сүйреп әкеліңіз\n", ConsoleColor.Green);

            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }

            string sourceText = File.ReadAllText(sourcePath);
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = "test.exe";
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, sourceText);
            
            Attempt attempt = model.AddAttemption(CurrentUser, problem);
            
            if (results.Errors.HasErrors)
            {
                view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
            }
            else
            {
                RunSolution(problem, results.PathToAssembly, ref attempt);
            }
            model.Attempts.Add(attempt);
            ReadKey();
        }

        private void RunSolution(Problem problem, string assembly, ref Attempt attempt)
        {
            Verdict verdict = Verdict.Wrong_answer;
            foreach (var testCase in problem.TestCases)
            {
                Process process = new Process();
                process.StartInfo.FileName = assembly;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.ErrorDialog = false;
            
                process.Start();
            
                StreamWriter stdInputWriter  = process.StandardInput;
                StreamReader stdOutputReader  = process.StandardOutput;
                
                stdInputWriter.WriteLine(testCase.Input);
                string res = stdOutputReader.ReadToEnd();
                
                if (string.Compare(res.Trim(), testCase.Output, StringComparison.InvariantCultureIgnoreCase)==0)
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
            if(verdict == Verdict.Accepted)
                view.Print("Дұрыс жауап!\n", ConsoleColor.Green);
            else
            {
                view.Print("Қате жауап!\n", ConsoleColor.Red);
            }
            ReadKey();
        }
    }
}