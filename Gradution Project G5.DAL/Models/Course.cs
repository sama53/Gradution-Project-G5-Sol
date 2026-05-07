using Gradution_Project_G5.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.DAL.Entities
{

    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Categories Category { get; set; }
        public bool IsActive { get; set; } = true;

        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
