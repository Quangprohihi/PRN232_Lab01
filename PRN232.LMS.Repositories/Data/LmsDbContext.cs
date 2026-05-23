using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Semester> Semesters => Set<Semester>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Subject> Subjects => Set<Subject>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var semesters = new List<Semester>();
        for (var i = 1; i <= 5; i++)
        {
            semesters.Add(new Semester
            {
                SemesterId = i,
                SemesterName = $"Semester {i}",
                StartDate = new DateTime(2024, 1, 1).AddMonths((i - 1) * 4),
                EndDate = new DateTime(2024, 4, 30).AddMonths((i - 1) * 4)
            });
        }

        var subjects = new List<Subject>();
        for (var i = 1; i <= 10; i++)
        {
            subjects.Add(new Subject
            {
                SubjectId = i,
                SubjectCode = $"SUB{i:000}",
                SubjectName = $"Subject {i}",
                Credit = i % 3 + 2
            });
        }

        var courses = new List<Course>();
        for (var i = 1; i <= 20; i++)
        {
            courses.Add(new Course
            {
                CourseId = i,
                CourseName = $"Course {i}",
                SubjectId = (i - 1) % 10 + 1,
                SemesterId = (i - 1) % 5 + 1
            });
        }

        var students = new List<Student>();
        for (var i = 1; i <= 50; i++)
        {
            students.Add(new Student
            {
                StudentId = i,
                FullName = $"Student {i}",
                Email = $"student{i}@lms.local",
                DateOfBirth = new DateTime(2000, 1, 1).AddDays(i * 30)
            });
        }

        var statuses = new[] { "Active", "Completed", "Dropped" };
        var enrollments = new List<Enrollment>();
        for (var i = 1; i <= 500; i++)
        {
            enrollments.Add(new Enrollment
            {
                EnrollmentId = i,
                StudentId = (i - 1) % 50 + 1,
                CourseId = (i - 1) % 20 + 1,
                EnrollDate = new DateTime(2024, 1, 1).AddDays(i % 180),
                Status = statuses[(i - 1) % statuses.Length]
            });
        }

        modelBuilder.Entity<Semester>().HasData(semesters);
        modelBuilder.Entity<Subject>().HasData(subjects);
        modelBuilder.Entity<Course>().HasData(courses);
        modelBuilder.Entity<Student>().HasData(students);
        modelBuilder.Entity<Enrollment>().HasData(enrollments);
    }
}
