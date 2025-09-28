using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.ViewModels.GradesVM
{
    public class GradeCreateVM
    {
        [Required(ErrorMessage = "Session is required")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Trainee is required")]
        public int TraineeId { get; set; }

        [Required(ErrorMessage = "Grade value is required")]
        [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
        public int Value { get; set; }
    }
}