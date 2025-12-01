using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.DTOs.Posts;
using VietCommerce.Core.Exceptions;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// ✅ SECURE Implementation của IPostService
    /// - Sử dụng ICurrentUser để lấy CustomerId trực tiếp từ JWT token
    /// - Validate permissions và ownership
    /// </summary>
    public class PostService : BaseService, IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        // Cache TTL constants
        private static readonly TimeSpan FeedCacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan DetailCacheDuration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan CustomerPostsCacheDuration = TimeSpan.FromMinutes(15);

        // File upload constants
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxImageSize = 5 * 1024 * 1024; // 5MB

        public PostService(
            IUnitOfWork unitOfWork,
            IFileUploadService fileUploadService,
            IMapper mapper,
            ICurrentUser currentUser,
            ILogger<PostService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        #region Create Post
        public async Task<ApiResponse<PostResponseDto>> CreatePostAsync(CreatePostDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ===== 1️⃣ Authentication & Permission =====
                var customerId = GetCurrentCustomerId();

                if (!await _currentUser.HasPermissionAsync("post:create"))
                    throw new UnauthorizedAccessException("You don't have permission to create posts");

                // ===== 2️⃣ Validation =====
                ValidateNotNull(dto, nameof(dto));

                if (string.IsNullOrWhiteSpace(dto.Content) && dto.PhotoFile == null)
                    throw new InvalidPostContentException("Post must have either content or photo");

                if (!string.IsNullOrWhiteSpace(dto.Content) && dto.Content.Length > 5000)
                    throw new InvalidPostContentException("Content cannot exceed 5000 characters");

                // ===== 3️⃣ Upload photo (if exists) =====
                string? photoPath = null;
                string? photoUrl = null;
                string? thumbnailUrl = null;
                string? photoHash = null;

                if (dto.PhotoFile != null)
                {
                    var uploadResult = await _fileUploadService.SaveFilesAsync(
                        files: new[] { dto.PhotoFile },
                        allowedExtensions: AllowedImageExtensions,
                        maxFileSize: MaxImageSize,
                        baseFolderPaths: new[] { "uploads", "posts" }
                    );

                    if (!uploadResult.Success || uploadResult.Data == null || !uploadResult.Data.Any())
                        throw new InvalidOperationException($"Failed to upload photo: {uploadResult.Message}");

                    var file = uploadResult.Data.First();
                    photoPath = file.FilePath;
                    photoUrl = file.FileUrl;           // URL lưu vào entity
                    thumbnailUrl = file.ThumbnailUrl;

                    photoHash = await ComputePhotoHashAsync(photoPath);

                    if (await _unitOfWork.Posts.IsDuplicateImageAsync(photoHash))
                        LogWarning($"⚠️ Duplicate image detected: {photoHash}");
                }

                // ===== 4️⃣ Create Post Entity =====
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Content = dto.Content?.Trim(),
                    PhotoPath = photoPath,
                    PhotoUrl = photoUrl,
                    PhotoHash = photoHash,
                    PostedOn = DateTime.UtcNow,
                    NotificationOn = dto.NotificationOn,
                    IsActive = true,
                    CreatedBy = customerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Posts.AddAsync(post);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Created post {post.Id} by customer {customerId}");

                // ===== 5️⃣ Cache Invalidation =====
                await InvalidateCacheByPrefixAsync("posts:feed:*");
                await InvalidateCacheByPrefixAsync($"posts:customer:{customerId}:*");

                // ===== 6️⃣ Publish Event =====
                await PublishCacheInvalidationAsync("post:created", new { PostId = post.Id, CustomerId = customerId });

                // ===== 7️⃣ Map to DTO =====
                var responseDto = await MapToPostResponseDtoAsync(post, customerId);

                return responseDto;

            }, "CreatePost", "Post created successfully");
        }

        #endregion

        #region Update Post
        public async Task<ApiResponse<PostResponseDto>> UpdatePostAsync(Guid postId, UpdatePostDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== AUTHENTICATION & AUTHORIZATION ==========
                var customerId = GetCurrentCustomerId(); // ✅ Sync, không await

                // ========== VALIDATION ==========
                ValidateId(postId, nameof(postId));
                ValidateNotNull(dto, nameof(dto));

                // Validate content length
                if (!string.IsNullOrWhiteSpace(dto.Content) && dto.Content.Length > 5000)
                {
                    throw new InvalidPostContentException("Content cannot exceed 5000 characters");
                }

                // ========== GET POST & CHECK OWNERSHIP ==========
                var post = await GetPostAndCheckOwnershipAsync(postId, customerId);

                // ========== UPDATE CONTENT ==========
                if (!string.IsNullOrWhiteSpace(dto.Content))
                {
                    post.Content = dto.Content.Trim();
                }

                // ========== HANDLE PHOTO ==========
                string? oldPhotoPath = post.PhotoPath;

                // Remove photo nếu requested
                if (dto.RemovePhoto)
                {
                    post.PhotoPath = null;
                    post.PhotoUrl = null;
                    post.PhotoHash = null;
                }
                // Upload new photo nếu có
                else if (dto.PhotoFile != null)
                {
                    var uploadResult = await _fileUploadService.SaveFilesAsync(
                        files: new[] { dto.PhotoFile },
                        allowedExtensions: AllowedImageExtensions,
                        maxFileSize: MaxImageSize,
                        baseFolderPaths: new[] { "uploads", "posts" }
                    );

                    if (!uploadResult.Success || uploadResult.Data == null || !uploadResult.Data.Any())
                    {
                        throw new InvalidOperationException($"Failed to upload photo: {uploadResult.Message}");
                    }

                    var file = uploadResult.Data.First();
                    post.PhotoPath = file.FilePath;
                    post.PhotoUrl = file.FileUrl;
                    post.PhotoHash = await ComputePhotoHashAsync(file.FilePath);
                }

                // ========== UPDATE METADATA ==========
                post.UpdatedBy = customerId;
                post.UpdatedAt = DateTime.UtcNow;
                post.RowVersion = dto.RowVersion; // For concurrency check

                // ========== SAVE CHANGES ==========
                try
                {
                    _unitOfWork.Posts.Update(post);
                    await _unitOfWork.SaveChangesAsync();

                    // Delete old photo file nếu có photo mới hoặc remove photo
                    if (!string.IsNullOrEmpty(oldPhotoPath) && oldPhotoPath != post.PhotoPath)
                    {
                        await _fileUploadService.DeleteFileAsync(oldPhotoPath);
                    }

                    LogInfo($"✅ Updated post {postId}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new PostConcurrencyException(postId);
                }

                // ========== CACHE INVALIDATION ==========
                await InvalidateMultipleCachesAsync(
                    $"posts:detail:{postId}",
                    $"posts:stats:{postId}"
                );
                await InvalidateCacheByPrefixAsync($"posts:customer:{customerId}:*");
                await InvalidateCacheByPrefixAsync("posts:feed:*");

                // ========== MAP TO DTO ==========
                var responseDto = await MapToPostResponseDtoAsync(post, customerId);

                return responseDto;

            }, "UpdatePost", "Post updated successfully");
        }
        #endregion

        #region Delete Post
        public async Task<ApiResponse<bool>> DeletePostAsync(Guid postId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== AUTHENTICATION & AUTHORIZATION ==========
                var customerId = GetCurrentCustomerId(); // ✅ Sync, không await

                // ========== VALIDATION ==========
                ValidateId(postId, nameof(postId));

                // ========== GET POST & CHECK OWNERSHIP ==========
                var post = await GetPostAndCheckOwnershipAsync(postId, customerId);

                // ========== SOFT DELETE ==========
                post.IsDeleted = true;
                post.DeletedAt = DateTime.UtcNow;
                post.DeletedBy = customerId;

                _unitOfWork.Posts.Update(post);
                await _unitOfWork.SaveChangesAsync();

                // ========== DELETE PHYSICAL FILE ==========
                if (!string.IsNullOrEmpty(post.PhotoPath))
                {
                    await _fileUploadService.DeleteFileAsync(post.PhotoPath);
                }

                LogInfo($"✅ Deleted post {postId}");

                // ========== CACHE INVALIDATION ==========
                await InvalidateCacheByPrefixAsync($"posts:*:{postId}:*");
                await InvalidateCacheByPrefixAsync($"posts:customer:{customerId}:*");
                await InvalidateCacheByPrefixAsync("posts:feed:*");

                // ========== PUBLISH EVENT ==========
                await PublishCacheInvalidationAsync("post:deleted", new { PostId = postId });

                return true;

            }, "DeletePost", "Post deleted successfully");
        }
        #endregion

        #region Get Post Feed
        public async Task<ApiResponse<PaginatedResult<PostFeedDto>>> GetPostFeedAsync(
            int pageNumber = 1,
            int pageSize = 20)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== VALIDATION ==========
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                // Get current customer (nullable cho guest)
                var currentCustomerId = GetCurrentCustomerIdOrNull(); // ✅ Sync, không await

                // ========== CACHE KEY ==========
                var cacheKey = CreateCacheKey("posts:feed", pageNumber, pageSize);

                // ========== GET FROM CACHE OR DB ==========
                var result = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    // 1. Fetch posts
                    var pagedPosts = await _unitOfWork.Posts.GetPostFeedAsync(
                        pageNumber,
                        pageSize,
                        currentCustomerId);

                    var postIds = pagedPosts.Items.Select(p => p.Id).ToList();

                    // 2. Batch fetch likes & bookmarks
                    HashSet<Guid> likedPostIds = new();
                    HashSet<Guid> bookmarkedPostIds = new();

                    if (currentCustomerId.HasValue && postIds.Any())
                    {
                        likedPostIds = await _unitOfWork.Posts
                            .GetLikedPostIdsByCustomerAsync(postIds, currentCustomerId.Value);

                        bookmarkedPostIds = await _unitOfWork.Posts
                            .GetBookmarkedPostIdsByCustomerAsync(postIds, currentCustomerId.Value);
                    }

                    // 3. Map với sync enrich
                    var dtos = pagedPosts.Items.Select(post =>
                    {
                        var dto = _mapper.Map<PostFeedDto>(post);
                        EnrichPostDto(dto, post, currentCustomerId, likedPostIds, bookmarkedPostIds); // ← Dùng sync version
                        return dto;
                    }).ToList();

                    return new PaginatedResult<PostFeedDto>(
                        dtos,
                        pagedPosts.PageNumber,
                        pagedPosts.PageSize,
                        pagedPosts.TotalItems
                    );
                }, FeedCacheDuration);

                return result;
            }, "GetPostFeed", "Retrieved post feed successfully");
        }
        #endregion

        #region Get Post By ID
        public async Task<ApiResponse<PostDetailDto>> GetPostByIdAsync(Guid postId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== VALIDATION ==========
                ValidateId(postId, nameof(postId));

                // Get current customer (nullable cho guest)
                var currentCustomerId = GetCurrentCustomerIdOrNull(); // ✅ Sync, không await

                // ========== CACHE KEY ==========
                var cacheKey = CreateCacheKey("posts:detail", postId);

                // ========== GET FROM CACHE OR DB ==========
                var result = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var post = await _unitOfWork.Posts.GetPostWithDetailsAsync(postId);

                    if (post == null)
                    {
                        throw new PostNotFoundException(postId);
                    }

                    // Map to DTO
                    var dto = _mapper.Map<PostDetailDto>(post);
                    await EnrichPostDtoAsync(dto, post, currentCustomerId);

                    // Get recent likes (top 10 customers)
                    var likerIds = await _unitOfWork.Posts.GetPostLikersAsync(postId, 10);
                    var likerDtos = new List<CustomerDto>();

                    foreach (var likerId in likerIds)
                    {
                        var customer = await _unitOfWork.Customers.GetByIdAsync(likerId);
                        if (customer != null)
                        {
                            likerDtos.Add(_mapper.Map<CustomerDto>(customer));
                        }
                    }
                    dto.RecentLikes = likerDtos;

                    return dto;

                }, DetailCacheDuration);

                return result;

            }, "GetPostById", "Retrieved post successfully");
        }
        #endregion

        #region Get Posts By Customer
        public async Task<ApiResponse<PaginatedResult<PostResponseDto>>> GetPostsByCustomerIdAsync(
            Guid customerId,
            int pageNumber = 1,
            int pageSize = 20)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== VALIDATION ==========
                ValidateId(customerId, nameof(customerId));
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                // ========== CACHE KEY ==========
                var cacheKey = CreateCacheKey("posts:customer", customerId, pageNumber, pageSize);

                // ========== GET FROM CACHE OR DB ==========
                var result = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var pagedPosts = await _unitOfWork.Posts.GetPostsByCustomerIdAsync(
                        customerId,
                        pageNumber,
                        pageSize);

                    // Map to DTOs
                    var dtoTasks = pagedPosts.Items.Select(async post =>
                    {
                        var dto = _mapper.Map<PostResponseDto>(post);
                        await EnrichPostDtoAsync(dto, post, customerId);
                        return dto;
                    });

                    var dtos = await Task.WhenAll(dtoTasks);

                    return new PaginatedResult<PostResponseDto>(
                        dtos,
                        pagedPosts.PageNumber,
                        pagedPosts.PageSize,
                        pagedPosts.TotalItems
                    );

                }, CustomerPostsCacheDuration);

                return result;

            }, "GetPostsByCustomerId", "Retrieved customer posts successfully");
        }
        #endregion

        #region Search Posts
        public async Task<ApiResponse<PaginatedResult<PostResponseDto>>> SearchPostsAsync(
            string keyword,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== VALIDATION ==========
                if (string.IsNullOrWhiteSpace(keyword) && !fromDate.HasValue && !toDate.HasValue)
                {
                    throw new ArgumentException("At least one search criteria must be provided");
                }

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                // ========== SEARCH ==========
                var pagedPosts = await _unitOfWork.Posts.SearchPostsAsync(
                    keyword,
                    fromDate,
                    toDate,
                    pageNumber,
                    pageSize);

                // Map to DTOs
                var dtoTasks = pagedPosts.Items.Select(async post =>
                {
                    var dto = _mapper.Map<PostResponseDto>(post);
                    await EnrichPostDtoAsync(dto, post, null);
                    return dto;
                });

                var dtos = await Task.WhenAll(dtoTasks);

                return new PaginatedResult<PostResponseDto>(
                    dtos,
                    pagedPosts.PageNumber,
                    pagedPosts.PageSize,
                    pagedPosts.TotalItems
                );

            }, "SearchPosts", "Search completed successfully");
        }
        #endregion

        #region Toggle Like
        public async Task<ApiResponse<PostLikeResult>> ToggleLikeAsync(Guid postId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== AUTHENTICATION ==========
                var customerId = GetCurrentCustomerId(); // ✅ Sync, không await

                // ========== VALIDATION ==========
                ValidateId(postId, nameof(postId));

                // Check post exists
                var post = await _unitOfWork.Posts.GetByIdAsync(postId);
                if (post == null || post.IsDeleted)
                {
                    throw new PostNotFoundException(postId);
                }

                // ========== CHECK EXISTING LIKE ==========
                var isLiked = await _unitOfWork.Likes.ExistsAsync(postId, customerId);

                bool liked;

                if (isLiked)
                {
                    // Unlike
                    var deleted = await _unitOfWork.Likes.DeleteAsync(postId, customerId);

                    if (deleted)
                    {
                        await _unitOfWork.SaveChangesAsync();
                        LogInfo($"🤍 Customer {customerId} unliked post {postId}");
                    }

                    liked = false;
                }
                else
                {
                    // Like
                    var like = new Like
                    {
                        PostId = postId,
                        CustomerId = customerId,
                        LikedOn = DateTime.UtcNow
                    };

                    await _unitOfWork.Likes.AddAsync(like);
                    await _unitOfWork.SaveChangesAsync();

                    liked = true;
                    LogInfo($"❤️ Customer {customerId} liked post {postId}");
                }

                // ========== COUNT TOTAL LIKES ==========
                var totalLikes = await _unitOfWork.Likes.CountByPostIdAsync(postId);

                // ========== CACHE INVALIDATION ==========
                await InvalidateCacheAsync($"posts:stats:{postId}");
                await InvalidateCacheAsync($"posts:detail:{postId}");
                // NEW: invalidate feed pages so clients will see updated isLiked/isBookmarked on reload
                await InvalidateCacheByPrefixAsync("posts:feed:*");

                // Optionally invalidate customer feed / lists if relevant
                await InvalidateCacheByPrefixAsync($"posts:customer:{post.CustomerId}:*");

                // Publish event for distributed cache or real-time sync
                await PublishCacheInvalidationAsync("post:liked", new { PostId = postId, CustomerId = customerId });
                return new PostLikeResult
                {
                    Liked = liked,
                    TotalLikes = totalLikes
                };

            }, "ToggleLike", "Like toggled successfully");
        }
        #endregion

        #region Toggle Bookmark
        public async Task<ApiResponse<PostBookmarkResult>> ToggleBookmarkAsync(Guid postId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                // ========== AUTH ==========
                var customerId = GetCurrentCustomerId(); // ✅ Sync, không await

                // ========== VALIDATION ==========
                ValidateId(postId, nameof(postId));

                var post = await _unitOfWork.Posts.GetByIdAsync(postId);
                if (post == null || post.IsDeleted)
                    throw new PostNotFoundException(postId);

                // ========== CHECK EXISTING BOOKMARK ==========
                var isBookmarked = await _unitOfWork.Bookmarks.ExistsAsync(postId, customerId);

                bool bookmarked;

                if (isBookmarked)
                {
                    // Remove bookmark
                    var deleted = await _unitOfWork.Bookmarks.DeleteAsync(postId, customerId);
                    if (deleted)
                    {
                        await _unitOfWork.SaveChangesAsync();
                        LogInfo($"🔖 Customer {customerId} unbookmarked post {postId}");
                    }
                    bookmarked = false;
                }
                else
                {
                    // Add bookmark
                    var bookmark = new Bookmark
                    {
                        PostId = postId,
                        CustomerId = customerId,
                        BookmarkedOn = DateTime.UtcNow
                    };

                    await _unitOfWork.Bookmarks.AddAsync(bookmark);
                    await _unitOfWork.SaveChangesAsync();

                    bookmarked = true;
                    LogInfo($"📌 Customer {customerId} bookmarked post {postId}");
                }

                // ========== COUNT TOTAL BOOKMARKS ==========
                var totalBookmarks = await _unitOfWork.Bookmarks.CountByPostIdAsync(postId);

                // ========== CACHE INVALIDATION ==========
                await InvalidateCacheAsync($"posts:stats:{postId}");
                await InvalidateCacheAsync($"posts:detail:{postId}");
                // NEW: invalidate feed pages
                await InvalidateCacheByPrefixAsync("posts:feed:*");
                await InvalidateCacheByPrefixAsync($"posts:customer:{post.CustomerId}:*");

                await PublishCacheInvalidationAsync("post:bookmarked", new { PostId = postId, CustomerId = customerId });
                return new PostBookmarkResult
                {
                    Bookmarked = bookmarked,
                    TotalBookmarks = totalBookmarks
                };

            }, "ToggleBookmark", "Bookmark toggled successfully");
        }
        #endregion

        #region Private Helper Methods

        /// <summary>
        /// ✅ Get CustomerId từ current user (REQUIRED - throw nếu không có)
        /// Đọc trực tiếp từ JWT token, không cần query database
        /// </summary>
        private Guid GetCurrentCustomerId()
        {
            var customerId = _currentUser.CustomerId;

            if (customerId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated or CustomerId not found in token");
            }

            return customerId;
        }

        /// <summary>
        /// ✅ Get CustomerId từ current user (NULLABLE - cho guest access)
        /// Đọc trực tiếp từ JWT token, không cần query database
        /// </summary>
        private Guid? GetCurrentCustomerIdOrNull()
        {
            var customerId = _currentUser.CustomerId;
            return customerId == Guid.Empty ? null : customerId;
        }

        /// <summary>
        /// Get post và kiểm tra ownership (authorization)
        /// </summary>
        private async Task<Post> GetPostAndCheckOwnershipAsync(Guid postId, Guid currentCustomerId)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(postId);

            if (post == null || post.IsDeleted)
            {
                throw new PostNotFoundException(postId);
            }

            if (post.CustomerId != currentCustomerId)
            {
                throw new UnauthorizedPostAccessException(postId, currentCustomerId);
            }

            return post;
        }

        /// <summary>
        /// Map Post entity sang PostResponseDto với đầy đủ thông tin
        /// </summary>
        private async Task<PostResponseDto> MapToPostResponseDtoAsync(Post post, Guid? currentCustomerId)
        {
            var dto = _mapper.Map<PostResponseDto>(post);
            await EnrichPostDtoAsync(dto, post, currentCustomerId);
            return dto;
        }

        /// <summary>
        /// Enrich DTO async - dùng cho single post operations
        /// </summary>
        private async Task EnrichPostDtoAsync(PostResponseDto dto, Post post, Guid? currentCustomerId)
        {
            if (currentCustomerId.HasValue)
            {
                dto.IsOwnedByCurrentUser = (post.CustomerId == currentCustomerId.Value);
                dto.IsLikedByCurrentUser = await _unitOfWork.Posts.IsLikedByCustomerAsync(post.Id, currentCustomerId.Value);
                dto.IsBookmarkedByCurrentUser = await _unitOfWork.Posts.IsBookmarkedByCustomerAsync(post.Id, currentCustomerId.Value);
            }
        }

        /// <summary>
        /// Enrich DTO sync - dùng cho batch operations với pre-fetched data
        /// </summary>
        private void EnrichPostDto(
            PostResponseDto dto,
            Post post,
            Guid? currentCustomerId,
            HashSet<Guid> likedPostIds,
            HashSet<Guid> bookmarkedPostIds)
        {
            if (currentCustomerId.HasValue)
            {
                dto.IsOwnedByCurrentUser = (post.CustomerId == currentCustomerId.Value);
                dto.IsLikedByCurrentUser = likedPostIds.Contains(post.Id);
                dto.IsBookmarkedByCurrentUser = bookmarkedPostIds.Contains(post.Id);
            }
        }

        /// <summary>
        /// Compute SHA256 hash của file ảnh
        /// </summary>
        private async Task<string> ComputePhotoHashAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return string.Empty;
            }

            using var sha256 = SHA256.Create();
            await using var stream = File.OpenRead(filePath);
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return Convert.ToBase64String(hashBytes);
        }

        #endregion
    }
}