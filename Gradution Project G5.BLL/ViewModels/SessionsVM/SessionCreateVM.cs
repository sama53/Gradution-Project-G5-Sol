using Gradution_Project_G5.BLL.Validation;
using Gradution_Project_G5.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.ViewModels.SessionsVM
{
    public class SessionCreateVM
    {
        [Required(ErrorMessage = "Session title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Start date cannot be in the past")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "End date cannot be in the past")]
        [DateAfter("StartDate", ErrorMessage = "End date must be after start date")]
        [MinDuration(ErrorMessage = "Session duration must be at least 1 hour.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }
    }
}