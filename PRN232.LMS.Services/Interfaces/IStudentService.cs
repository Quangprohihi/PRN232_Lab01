using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResultModel<StudentBusinessModel>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int size);

    Task<StudentBusinessModel?> GetStudentByIdAsync(int id);

    Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel student);
}
