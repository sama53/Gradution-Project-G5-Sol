using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private ICourseRepo? _courses;
        private ISessionRepo? _sessions;
        private IUserRepo? _users;
        private IGradeRepo? _grades;
        private IInstructorRepo? _instructors;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICourseRepo Courses => _courses ??= new CourseRepo(_context);
        public ISessionRepo Sessions => _sessions ??= new SessionRepo(_context);
        public IUserRepo Users => _users ??= new UserRepo(_context);
        public IGradeRepo Grades => _grades ??= new GradeRepo(_context);
        public IInstructorRepo Instructors => _instructors ??= new InstructorRepo(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Complete() => _context.SaveChanges();


      
    }
}