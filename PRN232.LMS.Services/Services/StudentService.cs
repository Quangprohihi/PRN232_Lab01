using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Services;

public class StudentService : IStudentService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<PagedResultModel<StudentBusinessModel>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int size)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var (sortBy, sortDescending) = NormalizeSort(sort);
        var query = ApplySearch(_studentRepository.GetQuery(), search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var students = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<StudentBusinessModel>
        {
            Items = students.Select(MapToBusinessModel).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<StudentBusinessModel?> GetStudentByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        return student is null ? null : MapToBusinessModel(student);
    }

    public async Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel student)
    {
        var createdStudent = await _studentRepository.AddAsync(new Student
        {
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth
        });

        return MapToBusinessModel(createdStudent);
    }

    private static IQueryable<Student> ApplySearch(IQueryable<Student> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(student => EF.Functions.ILike(student.FullName, $"%{keyword}%"));
    }

    private static IQueryable<Student> ApplySort(
        IQueryable<Student> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "dateOfBirth" => sortDescending
                ? query.OrderByDescending(student => student.DateOfBirth)
                : query.OrderBy(student => student.DateOfBirth),
            "fullName" => sortDescending
                ? query.OrderByDescending(student => student.FullName)
                : query.OrderBy(student => student.FullName),
            _ => query.OrderBy(student => student.StudentId)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("fullName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "dateOfBirth" => (sortBy, sortDescending),
            "fullName" => (sortBy, sortDescending),
            _ => ("fullName", false)
        };
    }

    private static StudentBusinessModel MapToBusinessModel(Student student)
    {
        return new StudentBusinessModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth
        };
    }
}
