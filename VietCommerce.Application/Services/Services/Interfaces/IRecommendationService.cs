using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for the recommendation generation service
    /// Handles pattern matching, missing item identification, and audit logging
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Generates a recommendation based on user action sequence and cart items
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="actionSequence">The sequence of user actions</param>
        /// <param name="cartItems">The items currently in the user's cart</param>
        /// <returns>RecommendationPayload with matched ritual and missing items, or null if no match</returns>
        Task<RecommendationPayloadDto?> GenerateRecommendationAsync(
            Guid userId,
            string sessionId,
            List<ActionDto> actionSequence,
            List<CartItemDto> cartItems);

        /// <summary>
        /// Identifies missing items for a detected ritual based on cart contents
        /// </summary>
        /// <param name="ritual">The detected ritual</param>
        /// <param name="cartItems">The items currently in the user's cart</param>
        /// <returns>List of product IDs that are missing from the cart</returns>
        Task<List<Guid>> GetMissingItemsAsync(RitualDto ritual, List<CartItemDto> cartItems);

        /// <summary>
        /// Logs a recommendation for audit trail and analytics
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="payload">The recommendation payload to log</param>
        /// <returns>The ID of the created log entry</returns>
        Task<Guid> LogRecommendationAsync(Guid userId, string sessionId, RecommendationPayloadDto payload);

        /// <summary>
        /// Records user interaction with a recommendation
        /// </summary>
        /// <param name="logId">The recommendation log ID</param>
        /// <param name="interaction">The interaction type (viewed, dismissed, clicked)</param>
        /// <returns>True if successfully recorded, false otherwise</returns>
        Task<bool> RecordInteractionAsync(Guid logId, string interaction);

        /// <summary>
        /// Gets the product details for missing items
        /// </summary>
        /// <param name="missingItemIds">List of product IDs that are missing</param>
        /// <returns>List of product DTOs for the missing items</returns>
        Task<List<ProductListDto>> GetMissingItemDetailsAsync(List<Guid> missingItemIds);
    }
}
