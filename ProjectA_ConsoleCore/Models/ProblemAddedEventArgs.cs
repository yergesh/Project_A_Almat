namespace ProjectA_ConsoleCore.Models
{
    public class ProblemAddedEventArgs
    {
        public ProblemAddedEventArgs(string fromTeacherName, string fromTeacherLastName, string title, int point, string download, string deadline)
        {
            FromTeacherName = fromTeacherName;
            FromTeacherLastName = fromTeacherLastName;
            Title = title;
            Point = point;
            Download = download;
            Deadline = deadline;
        }
        public int Id { get; set; }
        public string FromTeacherName { get; set; }
        public string FromTeacherLastName { get; set; }
        public string Title { get; set; }
        public string Download { get; set; }
        public string Deadline { get; set; }

        public int Point { get; set; }

        // public ProblemAddedEventArgs()
        // {
        //     
        // }
    }
}