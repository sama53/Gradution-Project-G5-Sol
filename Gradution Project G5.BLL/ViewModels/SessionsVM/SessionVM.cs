namespace Gradution_Project_G5.BLL.ViewModels.SessionsVM
{
    public class SessionVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public double Duration { get; set; }
    }
}