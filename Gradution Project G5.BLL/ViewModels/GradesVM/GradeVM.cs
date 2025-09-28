namespace Gradution_Project_G5.BLL.ViewModels.GradesVM
{
    public class GradeVM
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int SessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
    }
}