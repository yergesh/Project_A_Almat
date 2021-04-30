using System;
using System.Text;

namespace ProjectA_Console
{
    internal class Program
    {

        public static void Main(string[] args)
        {      
            Console.OutputEncoding = Encoding.UTF8;
            Controller.Controller controller = new Controller.Controller();
            controller.Main();
        }
    }
}