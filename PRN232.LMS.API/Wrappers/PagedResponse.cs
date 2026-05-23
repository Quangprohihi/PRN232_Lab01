namespace PRN232.LMS.API.Wrappers;

public class PagedResponse<T> : ApiResponse<T>
{
    public PaginationMetadata Pagination { get; set; } = new();

    public PagedResponse()
    {
    }

    public PagedResponse(
        bool success,
        string message,
        T? data,
        PaginationMetadata pagination,
        object? errors = null)
        : base(success, message, data, errors)
    {
        Pagination = pagination;
    }
}
