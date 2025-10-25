namespace VietCommerce.Core.Models;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string[]? Errors { get; set; }
    private ApiResponse() { }
    // -----------------------
    // ? Factory methods
    // -----------------------
    public static ApiResponse<T> SuccessResponse(
        T data,
        string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }
    public static ApiResponse<T> FailureResponse(
        string message = "Failed",
        string[]? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Errors = errors ?? new[] { message }
        };
    }
}
