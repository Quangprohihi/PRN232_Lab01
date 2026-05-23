using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository
{
    IQueryable<Student> GetQuery();

    Task<Student?> GetByIdAsync(int id);

    Task<Student> AddAsync(Student student);
}
