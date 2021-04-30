using System;
using System.Text;

namespace ProjectA_ConsoleCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.Unicode;
            Controller.Controller controller = new Controller.Controller();
            controller.Main();
        }
    }
}