namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork 
    {
        ICourseRepo Courses { get; }
        ISessionRepo Sessions { get; }
        IUserRepo Users { get; }
        IGradeRepo Grades { get; }
        IInstructorRepo Instructors { get; }

        Task<int> SaveChangesAsync();
        void Complete();
    }
}