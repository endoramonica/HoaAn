using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services.Identity;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Implementation of IMarketingPostService
/// Handles all business logic for Admin Marketing Post Management
/// Uses JWT authentication to extract admin user information
/// </summary>
public class MarketingPostService : BaseService, IMarketingPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    // Cache TTL constants
    private static readonly TimeSpan PostCacheDuration = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan ListCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan StatsCacheDuration = TimeSpan.FromMinutes(15);

    public MarketingPostService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<MarketingPostService> logger,
        ICacheService cacheService)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    #region CRUD Operations

    /// <summary>
    /// Get paginated list of marketing posts with filtering and search
    /// Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6
    /// </summary>
    public async Task<ApiResponse<PaginatedResult<MarketingPostListDto>>> GetPostsAsync(
        GetMarketingPostsQuery query)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📋 Getting marketing posts - Page: {query.PageNumber}, Size: {query.PageSize}");

            // Validate pagination parameters
            if (query.PageNumber < 1)
            {
                throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
            }

            if (query.PageSize < 1 || query.PageSize > 100)
            {
                throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
            }

            // Get posts from repository with all filters
            var result = await _unitOfWork.MarketingPosts.GetPostsAsync(
                pageNumber: query.PageNumber,
                pageSize: query.PageSize,
                status: query.Status,
                productId: query.ProductId,
                platform: query.Platform,
                searchTerm: query.SearchTerm,
                displayLocation: query.DisplayLocation,
                isFeatured: query.IsFeatured,
                minPriorityScore: query.MinPriorityScore,
                fromDate: query.FromDate,
                toDate: query.ToDate,
                sortBy: query.SortBy,
                sortOrder: query.SortOrder,
                includeDeleted: query.IncludeDeleted
            );

            // Map to DTOs
            var dtos = _mapper.Map<List<MarketingPostListDto>>(result.Items);

            // Enrich with TaggedProduct for each post with a valid productId
            foreach (var dto in dtos)
            {
                if (dto.ProductId.HasValue)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId.Value);
                    dto.TaggedProduct = TaggedProductHelper.BuildTaggedProductDto(product);
                }
            }

            var paginatedResult = new PaginatedResult<MarketingPostListDto>
            {
                Items = dtos,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };

            LogInfo($"✅ Retrieved {dtos.Count} posts (Total: {result.TotalItems})");
            return paginatedResult;

        }, "GetPostsAsync", "Posts retrieved successfully");
    }

    /// <summary>
    /// Get marketing post by ID with full details
    /// Requirements: 14.1, 16.3, 16.5, 16.6
    /// </summary>
    public async Task<ApiResponse<MarketingPostDetailDto>> GetPostByIdAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🔍 Getting marketing post by ID: {id}");

            // Validate post ID
            ValidateId(id, nameof(id));

            // Retrieve post
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Map to detail DTO
            var dto = _mapper.Map<MarketingPostDetailDto>(post);

            // Enrich with TaggedProduct if productId exists (Requirements: 16.3, 16.5, 16.6)
            if (dto.ProductId.HasValue)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId.Value);
                dto.TaggedProduct = TaggedProductHelper.BuildTaggedProductDto(product);
            }

            LogInfo($"✅ Retrieved post: {post.Title}");
            return dto;

        }, "GetPostByIdAsync", "Post retrieved successfully");
    }

    /// <summary>
    /// Create a new marketing post
    /// Requirements: 1.1, 1.2, 1.4, 2.1, 9.2, 9.3, 10.1, 11.1, 11.4, 12.1, 12.2, 12.3
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> CreatePostAsync(
        CreateMarketingPostDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"➕ Creating new marketing post: {dto.Title}");

            // Validate admin authentication (Requirements: 11.1, 11.4)
            var adminId = GetCurrentAdminId();

            // Validate input data (Requirements: 12.1, 12.2, 12.3)
            ValidateNotEmpty(dto.Title, nameof(dto.Title));
            ValidateNotEmpty(dto.Content, nameof(dto.Content));

            // Validate hashtags format (Requirement: 12.3)
            if (dto.Hashtags != null && dto.Hashtags.Any())
            {
                foreach (var hashtag in dto.Hashtags)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(hashtag, @"^[a-zA-Z0-9_]+$"))
                    {
                        throw new ArgumentException($"Hashtag '{hashtag}' contains invalid characters. Only alphanumeric and underscores allowed.", nameof(dto.Hashtags));
                    }
                }
            }

            // Validate product exists if productId provided (Requirements: 1.4, 10.1)
            if (dto.ProductId.HasValue)
            {
                var productExists = await ValidateProductExists(dto.ProductId.Value);
                if (!productExists)
                {
                    throw new ArgumentException($"Product with ID '{dto.ProductId}' does not exist", nameof(dto.ProductId));
                }

                // Get product name for denormalization
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId.Value);
                dto.ProductId = product!.Id;
            }

            // Map DTO to entity
            var post = _mapper.Map<MarketingPost>(dto);

            // Set default values (Requirements: 1.2, 2.1)
            post.Id = Guid.NewGuid();
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            post.CreatedBy = adminId;
            post.Status = dto.Status; // Default is Draft from DTO
            post.Views = 0;
            post.Clicks = 0;
            post.Shares = 0;
            post.IsDeleted = false;
            post.IsActive = true;

            // Apply SEO metadata defaults (Requirements: 9.2, 9.3)
            if (string.IsNullOrEmpty(post.MetaTitle))
            {
                post.MetaTitle = dto.Title;
            }

            if (string.IsNullOrEmpty(post.MetaDescription))
            {
                post.MetaDescription = !string.IsNullOrEmpty(dto.ShortDescription)
                    ? dto.ShortDescription
                    : (dto.Content.Length > 160 ? dto.Content.Substring(0, 160) : dto.Content);
            }

            // Get product name if product is linked
            if (post.ProductId.HasValue)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(post.ProductId.Value);
                post.ProductName = product?.Name;
            }

            // Set IsFeatured based on PriorityScore (Requirement: 14.4)
            post.IsFeatured = post.PriorityScore > 80;

            // Save to database
            var createdPost = await _unitOfWork.MarketingPosts.CreateAsync(post);
            await _unitOfWork.SaveChangesAsync();

            // Map to response DTO
            var responseDto = _mapper.Map<MarketingPostResponseDto>(createdPost);

            LogInfo($"✅ Created marketing post: {createdPost.Id} - {createdPost.Title}");
            return responseDto;

        }, "CreatePostAsync", "Post created successfully");
    }

    /// <summary>
    /// Update an existing marketing post
    /// Requirements: 1.3, 1.4, 9.5, 11.1, 12.1, 12.2, 12.3, 14.4
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> UpdatePostAsync(
        Guid id,
        UpdateMarketingPostDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"✏️ Updating marketing post: {id}");

            // Validate admin authentication and authorization (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Retrieve existing post
            var existingPost = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (existingPost == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Validate input data (Requirements: 12.1, 12.2, 12.3)
            if (dto.Title != null)
            {
                ValidateNotEmpty(dto.Title, nameof(dto.Title));
            }

            if (dto.Content != null)
            {
                ValidateNotEmpty(dto.Content, nameof(dto.Content));
            }

            // Validate hashtags format (Requirement: 12.3)
            if (dto.Hashtags != null && dto.Hashtags.Any())
            {
                foreach (var hashtag in dto.Hashtags)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(hashtag, @"^[a-zA-Z0-9_]+$"))
                    {
                        throw new ArgumentException($"Hashtag '{hashtag}' contains invalid characters. Only alphanumeric and underscores allowed.", nameof(dto.Hashtags));
                    }
                }
            }

            // Validate product exists if productId changed (Requirements: 1.4, 10.1)
            if (dto.ProductId.HasValue && dto.ProductId.Value != existingPost.ProductId)
            {
                var productExists = await ValidateProductExists(dto.ProductId.Value);
                if (!productExists)
                {
                    throw new ArgumentException($"Product with ID '{dto.ProductId}' does not exist", nameof(dto.ProductId));
                }
            }

            // Update only provided fields
            _mapper.Map(dto, existingPost);

            // Refresh updatedAt timestamp (Requirement: 1.3)
            existingPost.UpdatedAt = DateTime.UtcNow;
            existingPost.UpdatedBy = adminId;

            // Update product name if product changed
            if (dto.ProductId.HasValue && dto.ProductId.Value != existingPost.ProductId)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId.Value);
                existingPost.ProductName = product?.Name;
            }

            // Update IsFeatured based on PriorityScore (Requirement: 14.4)
            if (dto.PriorityScore.HasValue)
            {
                existingPost.IsFeatured = dto.PriorityScore.Value > 80;
            }

            // Handle concurrency conflicts
            try
            {
                var updatedPost = await _unitOfWork.MarketingPosts.UpdateAsync(existingPost);
                await _unitOfWork.SaveChangesAsync();

                // Map to response DTO
                var responseDto = _mapper.Map<MarketingPostResponseDto>(updatedPost);

                LogInfo($"✅ Updated marketing post: {updatedPost.Id} - {updatedPost.Title}");
                return responseDto;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                LogWarning($"⚠️ Concurrency conflict updating post {id}: {ex.Message}");
                throw new InvalidOperationException("Post was modified by another user. Please refresh and try again.", ex);
            }

        }, "UpdatePostAsync", "Post updated successfully");
    }

    /// <summary>
    /// Soft delete a marketing post
    /// Requirements: 4.1, 4.2, 11.1
    /// </summary>
    public async Task<ApiResponse<bool>> DeletePostAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🗑️ Deleting marketing post: {id}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Soft delete (Requirements: 4.1, 4.2)
            var result = await _unitOfWork.MarketingPosts.DeleteAsync(id, adminId);
            await _unitOfWork.SaveChangesAsync();

            LogInfo($"✅ Soft deleted marketing post: {id} - {post.Title}");
            return result;

        }, "DeletePostAsync", "Post deleted successfully");
    }

    /// <summary>
    /// Restore a soft-deleted marketing post
    /// Requirements: 4.5, 11.1
    /// </summary>
    public async Task<ApiResponse<bool>> RestorePostAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"♻️ Restoring marketing post: {id}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists (includeDeleted = true)
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: true);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            if (!post.IsDeleted)
            {
                LogWarning($"⚠️ Marketing post is not deleted: {id}");
                throw new InvalidOperationException($"Marketing post with ID '{id}' is not deleted");
            }

            // Restore (Requirement: 4.5)
            var result = await _unitOfWork.MarketingPosts.RestoreAsync(id);
            await _unitOfWork.SaveChangesAsync();

            LogInfo($"✅ Restored marketing post: {id} - {post.Title}");
            return result;

        }, "RestorePostAsync", "Post restored successfully");
    }

    #endregion

    #region Publishing Operations

    /// <summary>
    /// Publish a draft or scheduled post
    /// Requirements: 2.2, 11.1
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> PublishPostAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📤 Publishing marketing post: {id}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Validate post is Draft or Scheduled (Requirement: 2.2)
            if (post.Status != MarketingPostStatus.Draft && post.Status != MarketingPostStatus.Scheduled)
            {
                LogWarning($"⚠️ Cannot publish post {id} - Current status: {post.Status}");
                throw new InvalidOperationException($"Cannot publish post with status '{post.Status}'. Only Draft or Scheduled posts can be published.");
            }

            // Change status to Published (Requirement: 2.2)
            post.Status = MarketingPostStatus.Published;

            // Set publishedDate to current timestamp (Requirement: 2.2)
            post.PublishedDate = DateTime.UtcNow;

            // Clear scheduledDate if exists (Requirement: 2.2)
            post.ScheduledDate = null;

            // Update metadata
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedBy = adminId;

            // Save changes
            var updatedPost = await _unitOfWork.MarketingPosts.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync();

            // Map to response DTO
            var responseDto = _mapper.Map<MarketingPostResponseDto>(updatedPost);

            LogInfo($"✅ Published marketing post: {id} - {post.Title}");
            return responseDto;

        }, "PublishPostAsync", "Post published successfully");
    }

    /// <summary>
    /// Schedule a post for future publishing
    /// Requirements: 2.3, 11.1, 12.4
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> SchedulePostAsync(
        Guid id,
        DateTime scheduledDate)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📅 Scheduling marketing post: {id} for {scheduledDate}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate scheduledDate is in the future (Requirements: 2.3, 12.4)
            if (scheduledDate <= DateTime.UtcNow)
            {
                LogWarning($"⚠️ Invalid scheduled date: {scheduledDate} - Must be in the future");
                throw new ArgumentException("Scheduled date must be in the future", nameof(scheduledDate));
            }

            // Validate post exists
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Change status to Scheduled (Requirement: 2.3)
            post.Status = MarketingPostStatus.Scheduled;

            // Set scheduledDate (Requirement: 2.3)
            post.ScheduledDate = scheduledDate;

            // Clear publishedDate if exists (Requirement: 2.3)
            post.PublishedDate = null;

            // Update metadata
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedBy = adminId;

            // Save changes
            var updatedPost = await _unitOfWork.MarketingPosts.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync();

            // Map to response DTO
            var responseDto = _mapper.Map<MarketingPostResponseDto>(updatedPost);

            LogInfo($"✅ Scheduled marketing post: {id} - {post.Title} for {scheduledDate}");
            return responseDto;

        }, "SchedulePostAsync", "Post scheduled successfully");
    }

    /// <summary>
    /// Unpublish a published post (change back to draft)
    /// Requirements: 2.4, 11.1
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> UnpublishPostAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📥 Unpublishing marketing post: {id}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Validate post is Published (Requirement: 2.4)
            if (post.Status != MarketingPostStatus.Published)
            {
                LogWarning($"⚠️ Cannot unpublish post {id} - Current status: {post.Status}");
                throw new InvalidOperationException($"Cannot unpublish post with status '{post.Status}'. Only Published posts can be unpublished.");
            }

            // Change status to Draft (Requirement: 2.4)
            post.Status = MarketingPostStatus.Draft;

            // Clear publishedDate (Requirement: 2.4)
            post.PublishedDate = null;

            // Update metadata
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedBy = adminId;

            // Save changes
            var updatedPost = await _unitOfWork.MarketingPosts.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync();

            // Map to response DTO
            var responseDto = _mapper.Map<MarketingPostResponseDto>(updatedPost);

            LogInfo($"✅ Unpublished marketing post: {id} - {post.Title}");
            return responseDto;

        }, "UnpublishPostAsync", "Post unpublished successfully");
    }

    #endregion

    #region Analytics Operations

    /// <summary>
    /// Increment views counter for a post
    /// Requirements: 5.1, 5.4, 5.5
    /// </summary>
    public async Task<ApiResponse<bool>> IncrementViewsAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"👁️ Incrementing views for post: {id}");

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists (Requirement: 5.1)
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Increment views counter atomically (Requirements: 5.1, 5.4, 5.5)
            // The repository uses raw SQL to ensure atomic increment and prevent race conditions
            // This ensures the counter never decreases
            var result = await _unitOfWork.MarketingPosts.IncrementViewsAsync(id);

            if (!result)
            {
                LogWarning($"⚠️ Failed to increment views for post: {id}");
                throw new InvalidOperationException($"Failed to increment views for post '{id}'");
            }

            LogInfo($"✅ Incremented views for post: {id}");
            return result;

        }, "IncrementViewsAsync", "Views incremented successfully");
    }

    /// <summary>
    /// Increment clicks counter for a post
    /// Requirements: 5.2, 5.4, 5.5
    /// </summary>
    public async Task<ApiResponse<bool>> IncrementClicksAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🖱️ Incrementing clicks for post: {id}");

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists (Requirement: 5.2)
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Increment clicks counter atomically (Requirements: 5.2, 5.4, 5.5)
            // The repository uses raw SQL to ensure atomic increment and prevent race conditions
            // This ensures the counter never decreases
            var result = await _unitOfWork.MarketingPosts.IncrementClicksAsync(id);

            if (!result)
            {
                LogWarning($"⚠️ Failed to increment clicks for post: {id}");
                throw new InvalidOperationException($"Failed to increment clicks for post '{id}'");
            }

            LogInfo($"✅ Incremented clicks for post: {id}");
            return result;

        }, "IncrementClicksAsync", "Clicks incremented successfully");
    }

    /// <summary>
    /// Increment shares counter for a post
    /// Requirements: 5.3, 5.4, 5.5
    /// </summary>
    public async Task<ApiResponse<bool>> IncrementSharesAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🔗 Incrementing shares for post: {id}");

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate post exists (Requirement: 5.3)
            var post = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (post == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Increment shares counter atomically (Requirements: 5.3, 5.4, 5.5)
            // The repository uses raw SQL to ensure atomic increment and prevent race conditions
            // This ensures the counter never decreases
            var result = await _unitOfWork.MarketingPosts.IncrementSharesAsync(id);

            if (!result)
            {
                LogWarning($"⚠️ Failed to increment shares for post: {id}");
                throw new InvalidOperationException($"Failed to increment shares for post '{id}'");
            }

            LogInfo($"✅ Incremented shares for post: {id}");
            return result;

        }, "IncrementSharesAsync", "Shares incremented successfully");
    }

    #endregion

    #region Utility Operations

    /// <summary>
    /// Duplicate an existing marketing post
    /// Creates a new post with copied content, appends "(Copy)" to title,
    /// sets status to Draft, and resets analytics counters
    /// Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 11.1
    /// </summary>
    public async Task<ApiResponse<MarketingPostResponseDto>> DuplicatePostAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📋 Duplicating marketing post: {id}");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate post ID
            ValidateId(id, nameof(id));

            // Validate source post exists (Requirement: 7.1)
            var sourcePost = await _unitOfWork.MarketingPosts.GetByIdAsync(id, includeDeleted: false);

            if (sourcePost == null)
            {
                LogWarning($"⚠️ Marketing post not found: {id}");
                throw new KeyNotFoundException($"Marketing post with ID '{id}' not found");
            }

            // Create new post with copied content (Requirement: 7.1)
            var duplicatePost = new MarketingPost
            {
                // Generate new ID and timestamps (Requirement: 7.5)
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = adminId,

                // Append " (Copy)" to title (Requirement: 7.2)
                Title = $"{sourcePost.Title} (Copy)",

                // Copy all content fields (Requirement: 7.1)
                Content = sourcePost.Content,
                ShortDescription = sourcePost.ShortDescription,
                ImageUrl = sourcePost.ImageUrl,
                ImageData = sourcePost.ImageData,
                ImageUrls = sourcePost.ImageUrls,
                ProductId = sourcePost.ProductId,
                ProductName = sourcePost.ProductName,
                Topic = sourcePost.Topic,
                Platform = sourcePost.Platform,
                Tone = sourcePost.Tone,
                Hashtags = sourcePost.Hashtags,
                PriorityScore = sourcePost.PriorityScore,
                DisplayLocation = sourcePost.DisplayLocation,
                IsFeatured = sourcePost.IsFeatured,
                MetaTitle = sourcePost.MetaTitle,
                MetaDescription = sourcePost.MetaDescription,
                MetaKeywords = sourcePost.MetaKeywords,
                FacebookPost = sourcePost.FacebookPost,
                InstagramPost = sourcePost.InstagramPost,
                TwitterPost = sourcePost.TwitterPost,
                LinkedInPost = sourcePost.LinkedInPost,

                // Set status to Draft (Requirement: 7.3)
                Status = MarketingPostStatus.Draft,
                ScheduledDate = null,
                PublishedDate = null,

                // Reset analytics counters to zero (Requirement: 7.4)
                Views = 0,
                Clicks = 0,
                Shares = 0,

                // Initialize flags
                IsDeleted = false,
                IsActive = true
            };

            // Save to database
            var createdPost = await _unitOfWork.MarketingPosts.CreateAsync(duplicatePost);
            await _unitOfWork.SaveChangesAsync();

            // Map to response DTO
            var responseDto = _mapper.Map<MarketingPostResponseDto>(createdPost);

            LogInfo($"✅ Duplicated marketing post: {createdPost.Id} - {createdPost.Title}");
            return responseDto;

        }, "DuplicatePostAsync", "Post duplicated successfully");
    }

    /// <summary>
    /// Get aggregate statistics for all marketing posts
    /// Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6
    /// </summary>
    public async Task<ApiResponse<MarketingPostStatisticsDto>> GetStatisticsAsync()
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo("📊 Getting marketing post statistics");

            // Calculate statistics from repository (excludes deleted posts)
            // Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6
            var statistics = await _unitOfWork.MarketingPosts.GetStatisticsAsync();

            LogInfo($"✅ Retrieved statistics - Total: {statistics.Total}, Published: {statistics.Published}, Draft: {statistics.Draft}, Scheduled: {statistics.Scheduled}");
            return statistics;

        }, "GetStatisticsAsync", "Statistics retrieved successfully");
    }

    /// <summary>
    /// Get all marketing posts linked to a specific product
    /// Requirements: 10.3
    /// </summary>
    public async Task<ApiResponse<List<MarketingPostListDto>>> GetPostsByProductAsync(
        Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🔗 Getting marketing posts for product: {productId}");

            // Validate product ID (Requirement: 10.3)
            ValidateId(productId, nameof(productId));

            // Retrieve all posts linked to product (Requirement: 10.3)
            var posts = await _unitOfWork.MarketingPosts.GetByProductIdAsync(productId);

            // Map to list DTOs
            var dtos = _mapper.Map<List<MarketingPostListDto>>(posts);

            LogInfo($"✅ Retrieved {dtos.Count} posts for product: {productId}");
            return dtos;

        }, "GetPostsByProductAsync", "Posts retrieved successfully");
    }

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Bulk delete multiple marketing posts (soft delete)
    /// Requirements: 11.1, 13.1, 13.4, 13.5
    /// </summary>
    public async Task<ApiResponse<BulkOperationResult>> BulkDeleteAsync(List<Guid> postIds)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🗑️ Bulk deleting {postIds.Count} marketing posts");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate input
            if (postIds == null || !postIds.Any())
            {
                throw new ArgumentException("Post IDs list cannot be empty", nameof(postIds));
            }

            var result = new BulkOperationResult
            {
                TotalRequested = postIds.Count,
                SuccessCount = 0,
                FailureCount = 0,
                Errors = new List<BulkOperationError>()
            };

            // Process each post ID in transaction (Requirements: 13.1, 13.4, 13.5)
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var postId in postIds)
                {
                    try
                    {
                        // Validate post exists
                        var post = await _unitOfWork.MarketingPosts.GetByIdAsync(postId, includeDeleted: false);

                        if (post == null)
                        {
                            result.FailureCount++;
                            result.Errors.Add(new BulkOperationError
                            {
                                PostId = postId,
                                ErrorMessage = $"Post with ID '{postId}' not found"
                            });
                            continue;
                        }

                        // Soft delete the post (Requirement: 13.1)
                        var deleteResult = await _unitOfWork.MarketingPosts.DeleteAsync(postId, adminId);

                        if (deleteResult)
                        {
                            result.SuccessCount++;
                            LogDebug($"✅ Deleted post: {postId}");
                        }
                        else
                        {
                            result.FailureCount++;
                            result.Errors.Add(new BulkOperationError
                            {
                                PostId = postId,
                                ErrorMessage = "Failed to delete post"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add(new BulkOperationError
                        {
                            PostId = postId,
                            ErrorMessage = ex.Message
                        });
                        LogWarning($"⚠️ Error deleting post {postId}: {ex.Message}");
                    }
                }

                // Save all changes in transaction (Requirement: 13.5)
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                LogInfo($"✅ Bulk delete completed - Success: {result.SuccessCount}, Failed: {result.FailureCount}");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError($"❌ Bulk delete transaction failed: {ex.Message}");
                throw;
            }

        }, "BulkDeleteAsync", "Bulk delete completed");
    }

    /// <summary>
    /// Bulk publish multiple draft marketing posts
    /// Requirements: 11.1, 13.3, 13.4, 13.5
    /// </summary>
    public async Task<ApiResponse<BulkOperationResult>> BulkPublishAsync(List<Guid> postIds)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"📤 Bulk publishing {postIds.Count} marketing posts");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate input
            if (postIds == null || !postIds.Any())
            {
                throw new ArgumentException("Post IDs list cannot be empty", nameof(postIds));
            }

            var result = new BulkOperationResult
            {
                TotalRequested = postIds.Count,
                SuccessCount = 0,
                FailureCount = 0,
                Errors = new List<BulkOperationError>()
            };

            // Process each post ID in transaction (Requirements: 13.3, 13.4, 13.5)
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var postId in postIds)
                {
                    try
                    {
                        // Validate post exists
                        var post = await _unitOfWork.MarketingPosts.GetByIdAsync(postId, includeDeleted: false);

                        if (post == null)
                        {
                            result.FailureCount++;
                            result.Errors.Add(new BulkOperationError
                            {
                                PostId = postId,
                                ErrorMessage = $"Post with ID '{postId}' not found"
                            });
                            continue;
                        }

                        // Publish only draft posts (Requirement: 13.3)
                        if (post.Status != MarketingPostStatus.Draft)
                        {
                            result.FailureCount++;
                            result.Errors.Add(new BulkOperationError
                            {
                                PostId = postId,
                                ErrorMessage = $"Post status is '{post.Status}'. Only Draft posts can be published."
                            });
                            continue;
                        }

                        // Change status to Published
                        post.Status = MarketingPostStatus.Published;
                        post.PublishedDate = DateTime.UtcNow;
                        post.ScheduledDate = null;
                        post.UpdatedAt = DateTime.UtcNow;
                        post.UpdatedBy = adminId;

                        await _unitOfWork.MarketingPosts.UpdateAsync(post);
                        result.SuccessCount++;
                        LogDebug($"✅ Published post: {postId}");
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add(new BulkOperationError
                        {
                            PostId = postId,
                            ErrorMessage = ex.Message
                        });
                        LogWarning($"⚠️ Error publishing post {postId}: {ex.Message}");
                    }
                }

                // Save all changes in transaction (Requirement: 13.5)
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                LogInfo($"✅ Bulk publish completed - Success: {result.SuccessCount}, Failed: {result.FailureCount}");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError($"❌ Bulk publish transaction failed: {ex.Message}");
                throw;
            }

        }, "BulkPublishAsync", "Bulk publish completed");
    }

    /// <summary>
    /// Bulk update status for multiple marketing posts
    /// Requirements: 11.1, 13.2, 13.4, 13.5
    /// </summary>
    public async Task<ApiResponse<BulkOperationResult>> BulkUpdateStatusAsync(
        List<Guid> postIds,
        MarketingPostStatus status)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            LogInfo($"🔄 Bulk updating status to {status} for {postIds.Count} marketing posts");

            // Validate admin authentication (Requirement: 11.1)
            var adminId = GetCurrentAdminId();

            // Validate input
            if (postIds == null || !postIds.Any())
            {
                throw new ArgumentException("Post IDs list cannot be empty", nameof(postIds));
            }

            var result = new BulkOperationResult
            {
                TotalRequested = postIds.Count,
                SuccessCount = 0,
                FailureCount = 0,
                Errors = new List<BulkOperationError>()
            };

            // Process each post ID in transaction (Requirements: 13.2, 13.4, 13.5)
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var postId in postIds)
                {
                    try
                    {
                        // Validate post exists
                        var post = await _unitOfWork.MarketingPosts.GetByIdAsync(postId, includeDeleted: false);

                        if (post == null)
                        {
                            result.FailureCount++;
                            result.Errors.Add(new BulkOperationError
                            {
                                PostId = postId,
                                ErrorMessage = $"Post with ID '{postId}' not found"
                            });
                            continue;
                        }

                        // Update status for all specified posts (Requirement: 13.2)
                        post.Status = status;
                        post.UpdatedAt = DateTime.UtcNow;
                        post.UpdatedBy = adminId;

                        // Handle status-specific logic
                        switch (status)
                        {
                            case MarketingPostStatus.Published:
                                post.PublishedDate = DateTime.UtcNow;
                                post.ScheduledDate = null;
                                break;
                            case MarketingPostStatus.Draft:
                                post.PublishedDate = null;
                                post.ScheduledDate = null;
                                break;
                            case MarketingPostStatus.Scheduled:
                                // Note: ScheduledDate should be set separately via SchedulePostAsync
                                // For bulk operations, we just set the status
                                post.PublishedDate = null;
                                break;
                        }

                        await _unitOfWork.MarketingPosts.UpdateAsync(post);
                        result.SuccessCount++;
                        LogDebug($"✅ Updated status for post: {postId}");
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add(new BulkOperationError
                        {
                            PostId = postId,
                            ErrorMessage = ex.Message
                        });
                        LogWarning($"⚠️ Error updating status for post {postId}: {ex.Message}");
                    }
                }

                // Save all changes in transaction (Requirement: 13.5)
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                LogInfo($"✅ Bulk status update completed - Success: {result.SuccessCount}, Failed: {result.FailureCount}");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError($"❌ Bulk status update transaction failed: {ex.Message}");
                throw;
            }

        }, "BulkUpdateStatusAsync", "Bulk status update completed");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get current admin user ID from JWT token
    /// Validates that user is authenticated and has admin role
    /// Requirements: 11.1, 11.4, 11.5
    /// </summary>
    /// <returns>Admin user ID</returns>
    /// <exception cref="UnauthorizedAccessException">If user is not authenticated or not an admin</exception>
    private Guid GetCurrentAdminId()
    {
        // Get user ID from JWT token
        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
        {
            LogWarning("⚠️ Unauthorized access attempt - No user ID in token");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        // Verify admin role
        if (!_currentUser.IsAdmin && !_currentUser.HasPermission("post:create"))
        {
            LogWarning($"⚠️ Unauthorized access attempt - User {userId} is not an admin");
            throw new UnauthorizedAccessException("Admin role required");
        }

        LogDebug($"✅ Admin authenticated: {userId}");
        return userId;
    }

    /// <summary>
    /// Validate that a product exists in the catalog
    /// Requirements: 1.4, 10.1
    /// </summary>
    /// <param name="productId">Product ID to validate</param>
    /// <returns>True if product exists, false otherwise</returns>
    private async Task<bool> ValidateProductExists(Guid productId)
    {
        ValidateId(productId, nameof(productId));

        var product = await _unitOfWork.Products.GetByIdAsync(productId);

        if (product == null || product.IsDeleted)
        {
            LogWarning($"⚠️ Product validation failed - Product {productId} not found or deleted");
            return false;
        }

        LogDebug($"✅ Product validated: {productId} - {product.Name}");
        return true;
    }

    #endregion
}
