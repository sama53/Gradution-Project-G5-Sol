using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.ViewModels.GradesVM
{
    public class GradeEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Grade value is required")]
        [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
        public int Value { get; set; }
    }
}