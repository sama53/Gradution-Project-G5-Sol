using Gradution_Project_G5.BLL.Validation;
using Gradution_Project_G5.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.ViewModels.CoursesVM
{
    public class CourseCreateVM
    {
        [Required(ErrorMessage = "Course name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [NotOnlyNumbersAndSpecialChars(ErrorMessage = "Course name must contain at least one letter")] public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public Categories Category { get; set; }

        public int? InstructorId { get; set; }
    }
}