using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository
{
    IQueryable<Course> GetQuery();

    Task<Course?> GetByIdAsync(int id);

    Task<Course> AddAsync(Course course);
}
