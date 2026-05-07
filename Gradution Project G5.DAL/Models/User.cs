using Gradution_Project_G5.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.DAL.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}