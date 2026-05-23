using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Services;

public class SubjectService : ISubjectService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<PagedResultModel<SubjectBusinessModel>> GetSubjectsAsync(
        string? search,
        string? sort,
        int page,
        int size)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = ApplySearch(_subjectRepository.GetQuery(), search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var subjects = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<SubjectBusinessModel>
        {
            Items = subjects.Select(MapToBusinessModel).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        return subject is null ? null : MapToBusinessModel(subject);
    }

    public async Task<SubjectBusinessModel> CreateSubjectAsync(SubjectBusinessModel subject)
    {
        var createdSubject = await _subjectRepository.AddAsync(new Subject
        {
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit
        });

        return MapToBusinessModel(createdSubject);
    }

    private static IQueryable<Subject> ApplySearch(IQueryable<Subject> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(subject =>
            EF.Functions.ILike(subject.SubjectName, $"%{keyword}%") ||
            EF.Functions.ILike(subject.SubjectCode, $"%{keyword}%"));
    }

    private static IQueryable<Subject> ApplySort(
        IQueryable<Subject> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "subjectCode" => sortDescending
                ? query.OrderByDescending(subject => subject.SubjectCode)
                : query.OrderBy(subject => subject.SubjectCode),
            "credit" => sortDescending
                ? query.OrderByDescending(subject => subject.Credit)
                : query.OrderBy(subject => subject.Credit),
            _ => sortDescending
                ? query.OrderByDescending(subject => subject.SubjectName)
                : query.OrderBy(subject => subject.SubjectName)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("subjectName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "subjectCode" => (sortBy, sortDescending),
            "subjectName" => (sortBy, sortDescending),
            "credit" => (sortBy, sortDescending),
            _ => ("subjectName", false)
        };
    }

    private static SubjectBusinessModel MapToBusinessModel(Subject subject)
    {
        return new SubjectBusinessModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit
        };
    }
}
