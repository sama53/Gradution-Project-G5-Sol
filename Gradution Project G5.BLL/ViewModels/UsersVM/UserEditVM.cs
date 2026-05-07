using Gradution_Project_G5.BLL.Validation;
using Gradution_Project_G5.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.ViewModels.UsersVM
{
    public class UserEditVM
    {
        public int Id { get; set; }
        public bool IsInstructor { get; set; } = false;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        [NoNumber(ErrorMessage = "Name cannot contain numbers")]
        [NoSpecialCharacters(ErrorMessage = "Name cannot contain special characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public UserRole Role { get; set; }
    }
}