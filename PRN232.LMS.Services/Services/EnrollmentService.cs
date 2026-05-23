using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Services;

public class EnrollmentService : IEnrollmentService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IEnrollmentRepository _enrollmentRepository;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<PagedResultModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var normalizedExpands = NormalizeExpands(expands);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = ApplyExpansion(_enrollmentRepository.GetQuery(), normalizedExpands);
        query = ApplySearch(query, search);

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var enrollments = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<EnrollmentBusinessModel>
        {
            Items = enrollments.Select(enrollment => MapToBusinessModel(enrollment, normalizedExpands)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, List<string> expands)
    {
        var normalizedExpands = NormalizeExpands(expands);
        var enrollment = await ApplyExpansion(_enrollmentRepository.GetQuery(), normalizedExpands)
            .FirstOrDefaultAsync(item => item.EnrollmentId == id);

        return enrollment is null ? null : MapToBusinessModel(enrollment, normalizedExpands);
    }

    public async Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel enrollment)
    {
        var createdEnrollment = await _enrollmentRepository.AddAsync(new Enrollment
        {
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status
        });

        return MapToBusinessModel(createdEnrollment, new HashSet<string>());
    }

    private static IQueryable<Enrollment> ApplyExpansion(
        IQueryable<Enrollment> query,
        ISet<string> expands)
    {
        if (expands.Contains("student"))
        {
            query = query.Include(enrollment => enrollment.Student);
        }

        if (expands.Contains("course"))
        {
            query = query.Include(enrollment => enrollment.Course);
        }

        return query;
    }

    private static IQueryable<Enrollment> ApplySearch(IQueryable<Enrollment> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(enrollment => EF.Functions.ILike(enrollment.Status, $"%{keyword}%"));
    }

    private static IQueryable<Enrollment> ApplySort(
        IQueryable<Enrollment> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "status" => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.Status)
                : query.OrderBy(enrollment => enrollment.Status),
            "enrollDate" => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.EnrollDate)
                : query.OrderBy(enrollment => enrollment.EnrollDate),
            _ => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.EnrollmentId)
                : query.OrderBy(enrollment => enrollment.EnrollmentId)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("enrollmentId", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "enrollmentId" => (sortBy, sortDescending),
            "enrollDate" => (sortBy, sortDescending),
            "status" => (sortBy, sortDescending),
            _ => ("enrollmentId", false)
        };
    }

    private static ISet<string> NormalizeExpands(IEnumerable<string> expands)
    {
        return expands
            .Where(expand => !string.IsNullOrWhiteSpace(expand))
            .Select(expand => expand.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    private static EnrollmentBusinessModel MapToBusinessModel(
        Enrollment enrollment,
        ISet<string> expands)
    {
        return new EnrollmentBusinessModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = expands.Contains("student") && enrollment.Student is not null
                ? MapStudent(enrollment.Student)
                : null,
            Course = expands.Contains("course") && enrollment.Course is not null
                ? MapCourse(enrollment.Course)
                : null
        };
    }

    private static StudentBusinessModel MapStudent(Student student)
    {
        return new StudentBusinessModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth
        };
    }

    private static CourseBusinessModel MapCourse(Course course)
    {
        return new CourseBusinessModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId
        };
    }
}
