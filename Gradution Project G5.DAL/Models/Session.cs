using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.DAL.Entities
{
    public class Session
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
