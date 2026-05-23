using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly LmsDbContext _context;

    public StudentRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Student> GetQuery()
    {
        return _context.Students.AsNoTracking();
    }

    public Task<Student?> GetByIdAsync(int id)
    {
        return _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.StudentId == id);
    }

    public async Task<Student> AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }
}
