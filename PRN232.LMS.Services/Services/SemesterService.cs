using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Services;

public class SemesterService : ISemesterService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly ISemesterRepository _semesterRepository;

    public SemesterService(ISemesterRepository semesterRepository)
    {
        _semesterRepository = semesterRepository;
    }

    public async Task<PagedResultModel<SemesterBusinessModel>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int size)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = ApplySearch(_semesterRepository.GetQuery(), search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var semesters = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<SemesterBusinessModel>
        {
            Items = semesters.Select(MapToBusinessModel).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<SemesterBusinessModel?> GetSemesterByIdAsync(int id)
    {
        var semester = await _semesterRepository.GetByIdAsync(id);
        return semester is null ? null : MapToBusinessModel(semester);
    }

    public async Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel semester)
    {
        var createdSemester = await _semesterRepository.AddAsync(new Semester
        {
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate
        });

        return MapToBusinessModel(createdSemester);
    }

    private static IQueryable<Semester> ApplySearch(IQueryable<Semester> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(semester => EF.Functions.ILike(semester.SemesterName, $"%{keyword}%"));
    }

    private static IQueryable<Semester> ApplySort(
        IQueryable<Semester> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "startDate" => sortDescending
                ? query.OrderByDescending(semester => semester.StartDate)
                : query.OrderBy(semester => semester.StartDate),
            "endDate" => sortDescending
                ? query.OrderByDescending(semester => semester.EndDate)
                : query.OrderBy(semester => semester.EndDate),
            _ => sortDescending
                ? query.OrderByDescending(semester => semester.SemesterName)
                : query.OrderBy(semester => semester.SemesterName)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("semesterName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "startDate" => (sortBy, sortDescending),
            "endDate" => (sortBy, sortDescending),
            "semesterName" => (sortBy, sortDescending),
            _ => ("semesterName", false)
        };
    }

    private static SemesterBusinessModel MapToBusinessModel(Semester semester)
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
