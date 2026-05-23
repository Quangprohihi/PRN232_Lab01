using PRN232.LMS.API.Wrappers;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Helpers;

public static class PaginationHelper
{
    public static PaginationMetadata Create<T>(PagedResultModel<T> result)
    {
        return new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}
