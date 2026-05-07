using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gradution_Project_G5.DAL.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Grade> Grades => Set<Grade>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    Id = 1,
                    FirstName = "Mohamed",
                    LastName = "Ahmed",
                    Email = "mohamed.ahmed@email.com",
                    Specialization = Specialization.SoftwareDevelopment,
                    IsActive = true
                },
                new Instructor
                {
                    Id = 2,
                    FirstName = "Shrouk",
                    LastName = "Kandeel",
                    Email = "shrouk.kandeel@email.com",
                    Specialization = Specialization.Marketing,
                    IsActive = true
                }
            );

            // Course-Instructor relationship
            modelBuilder.Entity<Course>()
    .HasOne(c => c.Instructor)
    .WithMany(i => i.Courses)
    .HasForeignKey(c => c.InstructorId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.Restrict);

            // Session-Course relationship
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite Key for Grade (TraineeId + SessionId)
            modelBuilder.Entity<Grade>()
                .HasIndex(g => new { g.TraineeId, g.SessionId })
                .IsUnique();

            modelBuilder.Entity<Grade>()
                .HasKey(g => g.Id);

            // Grade → Session (One-to-Many)
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Session)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Grade → User (Trainee) (One-to-Many)
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Trainee)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}