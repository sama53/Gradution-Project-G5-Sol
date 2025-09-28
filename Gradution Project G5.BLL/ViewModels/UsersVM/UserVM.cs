using Gradution_Project_G5.DAL.Models;

namespace Gradution_Project_G5.BLL.ViewModels.UsersVM
{
    public class UserVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}