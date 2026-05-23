using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly LmsDbContext _context;

    public SubjectRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Subject> GetQuery()
    {
        return _context.Subjects.AsNoTracking();
    }

    public Task<Subject?> GetByIdAsync(int id)
    {
        return _context.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(subject => subject.SubjectId == id);
    }

    public async Task<Subject> AddAsync(Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return subject;
    }
}
