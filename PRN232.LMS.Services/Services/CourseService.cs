using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Services;

public class CourseService : ICourseService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<PagedResultModel<CourseBusinessModel>> GetCoursesAsync(
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

        var query = ApplyExpansion(_courseRepository.GetQuery(), normalizedExpands);
        query = ApplySearch(query, search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var courses = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<CourseBusinessModel>
        {
            Items = courses.Select(course => MapToBusinessModel(course, normalizedExpands)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<CourseBusinessModel?> GetCourseByIdAsync(int id, List<string> expands)
    {
        var normalizedExpands = NormalizeExpands(expands);
        var course = await ApplyExpansion(_courseRepository.GetQuery(), normalizedExpands)
            .FirstOrDefaultAsync(item => item.CourseId == id);

        return course is null ? null : MapToBusinessModel(course, normalizedExpands);
    }

    public async Task<CourseBusinessModel> CreateCourseAsync(CourseBusinessModel course)
    {
        var createdCourse = await _courseRepository.AddAsync(new Course
        {
            CourseName = course.CourseName,
            SubjectId = course.SubjectId,
            SemesterId = course.SemesterId
        });

        return MapToBusinessModel(createdCourse, new HashSet<string>());
    }

    private static IQueryable<Course> ApplyExpansion(
        IQueryable<Course> query,
        ISet<string> expands)
    {
        if (expands.Contains("subject"))
        {
            query = query.Include(course => course.Subject);
        }

        if (expands.Contains("semester"))
        {
            query = query.Include(course => course.Semester);
        }

        return query;
    }

    private static IQueryable<Course> ApplySearch(IQueryable<Course> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(course => EF.Functions.ILike(course.CourseName, $"%{keyword}%"));
    }

    private static IQueryable<Course> ApplySort(
        IQueryable<Course> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "subjectId" => sortDescending
                ? query.OrderByDescending(course => course.SubjectId)
                : query.OrderBy(course => course.SubjectId),
            "semesterId" => sortDescending
                ? query.OrderByDescending(course => course.SemesterId)
                : query.OrderBy(course => course.SemesterId),
            _ => sortDescending
                ? query.OrderByDescending(course => course.CourseName)
                : query.OrderBy(course => course.CourseName)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("courseName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "courseName" => (sortBy, sortDescending),
            "subjectId" => (sortBy, sortDescending),
            "semesterId" => (sortBy, sortDescending),
            _ => ("courseName", false)
        };
    }

    private static ISet<string> NormalizeExpands(IEnumerable<string> expands)
    {
        return expands
            .Where(expand => !string.IsNullOrWhiteSpace(expand))
            .Select(expand => expand.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    private static CourseBusinessModel MapToBusinessModel(
        Course course,
        ISet<string> expands)
    {
        return new CourseBusinessModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SubjectId = course.SubjectId,
            SemesterId = course.SemesterId,
            Subject = expands.Contains("subject") && course.Subject is not null
                ? MapSubject(course.Subject)
                : null,
            Semester = expands.Contains("semester") && course.Semester is not null
                ? MapSemester(course.Semester)
                : null
        };
    }

    private static SubjectBusinessModel MapSubject(Subject subject)
    {
        return new SubjectBusinessModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credits = subject.Credit
        };
    }

    private static SemesterBusinessModel MapSemester(Semester semester)
    {
        return new SemesterBusinessModel
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate
        };
    }
}
