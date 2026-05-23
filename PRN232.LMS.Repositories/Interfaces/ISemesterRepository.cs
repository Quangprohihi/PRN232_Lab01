using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository
{
    IQueryable<Semester> GetQuery();

    Task<Semester?> GetByIdAsync(int id);

    Task<Semester> AddAsync(Semester semester);
}
