using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    IQueryable<Enrollment> GetQuery();

    Task<Enrollment?> GetByIdAsync(int id);

    Task<Enrollment> AddAsync(Enrollment enrollment);
}
