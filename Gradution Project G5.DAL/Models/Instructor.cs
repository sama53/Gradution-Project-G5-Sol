using Gradution_Project_G5.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.DAL.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public Specialization Specialization { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}