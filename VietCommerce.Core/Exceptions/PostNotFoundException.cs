// VietCommerce.Core/Exceptions/PostExceptions.cs
namespace VietCommerce.Core.Exceptions;

/// <summary>
/// Exception khi không tìm thấy post
/// </summary>
public class PostNotFoundException : KeyNotFoundException
{
    public Guid PostId { get; }

    public PostNotFoundException(Guid postId)
        : base($"Post with ID {postId} not found or has been deleted")
    {
        PostId = postId;
    }

    public PostNotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception khi user không có quyền thao tác post
/// </summary>
public class UnauthorizedPostAccessException : UnauthorizedAccessException
{
    public Guid PostId { get; }
    public Guid CustomerId { get; }

    public UnauthorizedPostAccessException(Guid postId, Guid customerId)
        : base($"Customer {customerId} does not have permission to access Post {postId}")
    {
        PostId = postId;
        CustomerId = customerId;
    }

    public UnauthorizedPostAccessException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception khi phát hiện ảnh trùng lặp
/// </summary>
public class DuplicatePostException : InvalidOperationException
{
    public string PhotoHash { get; }

    public DuplicatePostException(string photoHash)
        : base($"A post with the same image (hash: {photoHash}) already exists")
    {
        PhotoHash = photoHash;
    }

    public DuplicatePostException(string message, string photoHash) : base(message)
    {
        PhotoHash = photoHash;
    }
}

/// <summary>
/// Exception khi xảy ra lỗi concurrency (RowVersion mismatch)
/// </summary>
public class PostConcurrencyException : InvalidOperationException
{
    public Guid PostId { get; }

    public PostConcurrencyException(Guid postId)
        : base($"Post {postId} was modified by another user. Please refresh and try again")
    {
        PostId = postId;
    }

    public PostConcurrencyException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception khi validation post content/photo thất bại
/// </summary>
public class InvalidPostContentException : ArgumentException
{
    public InvalidPostContentException(string message) : base(message)
    {
    }

    public InvalidPostContentException(string message, string paramName) : base(message, paramName)
    {
    }
}