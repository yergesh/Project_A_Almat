using System.Diagnostics;
using System.IO;

namespace ProjectA_ConsoleCore.Helper
{
    public class FileHelper
    {
        public string GetTextFromEditor()
        {
            string fileName =  Path.ChangeExtension(Path.GetTempFileName(), "txt");
            File.Create(fileName).Close();
            var process = Process.Start("notepad.exe", fileName);
            process.WaitForExit();
            return File.ReadAllText(fileName);
        }
    }
}