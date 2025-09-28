using Gradution_Project_G5.DAL.Models;

namespace Gradution_Project_G5.BLL.ViewModels.CoursesVM
{
    public class CourseVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Categories Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
    }
}