using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<PagedResultModel<SemesterBusinessModel>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int size);

    Task<SemesterBusinessModel?> GetSemesterByIdAsync(int id);

    Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel semester);
}
