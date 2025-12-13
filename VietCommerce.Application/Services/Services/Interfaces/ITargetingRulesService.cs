using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for campaign targeting rules
    /// Handles parsing, validation, and evaluation of targeting rules
    /// Requirements: 4.1, 4.2, 4.3, 4.4, 4.5
    /// </summary>
    public interface ITargetingRulesService
    {
        /// <summary>
        /// Parse and validate targeting rules from JSON string
        /// Requirements: 4.1
        /// </summary>
        /// <param name="rulesJson">JSON string containing targeting rules</param>
        /// <returns>Parsed TargetingRulesDto or null if parsing fails</returns>
        Task<TargetingRulesDto?> ParseAndValidateRulesAsync(string? rulesJson);

        /// <summary>
        /// Validate targeting rules DTO
        /// Requirements: 4.1
        /// </summary>
        /// <param name="rules">TargetingRulesDto to validate</param>
        /// <returns>List of validation errors, empty if valid</returns>
        Task<List<string>> ValidateRulesAsync(TargetingRulesDto? rules);

        /// <summary>
        /// Check if campaign should be displayed on a given page
        /// Requirements: 4.2
        /// </summary>
        /// <param name="rules">Targeting rules</param>
        /// <param name="currentPage">Current page name</param>
        /// <returns>True if campaign targets this page</returns>
        Task<bool> ShouldDisplayOnPageAsync(TargetingRulesDto? rules, string currentPage);

        /// <summary>
        /// Check if campaign should be displayed based on frequency rules
        /// Requirements: 4.3, 4.4, 4.5
        /// </summary>
        /// <param name="rules">Targeting rules</param>
        /// <param name="sessionId">User session ID</param>
        /// <param name="currentPage">Current page name</param>
        /// <param name="impressionHistory">List of pages where campaign was shown in this session</param>
        /// <returns>True if campaign should be displayed based on frequency</returns>
        Task<bool> ShouldDisplayBasedOnFrequencyAsync(
            TargetingRulesDto? rules,
            string sessionId,
            string currentPage,
            List<string> impressionHistory);

        /// <summary>
        /// Get delay before showing campaign
        /// Requirements: 4.4
        /// </summary>
        /// <param name="rules">Targeting rules</param>
        /// <returns>Delay in milliseconds</returns>
        Task<int> GetDisplayDelayAsync(TargetingRulesDto? rules);

        /// <summary>
        /// Get auto-dismiss delay for campaign
        /// Requirements: 4.5
        /// </summary>
        /// <param name="rules">Targeting rules</param>
        /// <returns>Auto-dismiss delay in milliseconds, 0 if no auto-dismiss</returns>
        Task<int> GetAutoDismissDelayAsync(TargetingRulesDto? rules);

        /// <summary>
        /// Evaluate all targeting rules to determine if campaign should be shown
        /// Requirements: 4.1, 4.2, 4.3, 4.4, 4.5
        /// </summary>
        /// <param name="rules">Targeting rules</param>
        /// <param name="sessionId">User session ID</param>
        /// <param name="currentPage">Current page name</param>
        /// <param name="impressionHistory">List of pages where campaign was shown in this session</param>
        /// <returns>True if all targeting conditions are met</returns>
        Task<bool> EvaluateTargetingRulesAsync(
            TargetingRulesDto? rules,
            string sessionId,
            string currentPage,
            List<string> impressionHistory);
    }
}
