using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LmsDbContext _context;

    public EnrollmentRepository(LmsDbContext context)
    {
        _context = context;
    }

    public IQueryable<Enrollment> GetQuery()
    {
        return _context.Enrollments.AsNoTracking();
    }

    public Task<Enrollment?> GetByIdAsync(int id)
    {
        return _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(enrollment => enrollment.EnrollmentId == id);
    }

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }
}
