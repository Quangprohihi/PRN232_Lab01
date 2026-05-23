namespace PRN232.LMS.API.Wrappers;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Errors { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data = default, object? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }
}
