using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly LmsDbContext _context;

    public SemesterRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Semester> GetQuery()
    {
        return _context.Semesters.AsNoTracking();
    }

    public Task<Semester?> GetByIdAsync(int id)
    {
        return _context.Semesters
            .AsNoTracking()
            .FirstOrDefaultAsync(semester => semester.SemesterId == id);
    }

    public async Task<Semester> AddAsync(Semester semester)
    {
        _context.Semesters.Add(semester);
        await _context.SaveChangesAsync();
        return semester;
    }
}
