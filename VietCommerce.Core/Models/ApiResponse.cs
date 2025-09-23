namespace VietCommerce.Core.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string[]? Errors { get; set; }

    public ApiResponse()
    {
        Success = true;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T> { Data = data, Message = message };
    }

    public static ApiResponse<object> ErrorResponse(string message, string[]? errors)
    {
        return new ApiResponse<object> { Success = false, Message = message, Errors = errors ?? new[] { message } };
    }
}