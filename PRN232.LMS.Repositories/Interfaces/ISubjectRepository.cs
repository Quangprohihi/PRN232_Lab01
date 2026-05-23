using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISubjectRepository
{
    IQueryable<Subject> GetQuery();

    Task<Subject?> GetByIdAsync(int id);

    Task<Subject> AddAsync(Subject subject);
}
