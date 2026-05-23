using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly LmsDbContext _context;

    public CourseRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Course> GetQuery()
    {
        return _context.Courses.AsNoTracking();
    }

    public Task<Course?> GetByIdAsync(int id)
    {
        return _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.CourseId == id);
    }

    public async Task<Course> AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }
}
