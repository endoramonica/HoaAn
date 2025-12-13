using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Helpers;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service implementation for campaign targeting rules
    /// Handles parsing, validation, and evaluation of targeting rules
    /// Requirements: 4.1, 4.2, 4.3, 4.4, 4.5
    /// </summary>
    public class TargetingRulesService : BaseService, ITargetingRulesService
    {
        // Valid frequency values
        private static readonly HashSet<string> ValidFrequencies = new()
        {
            "once-per-session",
            "once-per-page",
            "always"
        };

        public TargetingRulesService(
            ILogger<TargetingRulesService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
        }

        /// <summary>
        /// Parse and validate targeting rules from JSON string
        /// Requirements: 4.1
        /// </summary>
        public async Task<TargetingRulesDto?> ParseAndValidateRulesAsync(string? rulesJson)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo($"🔍 Parsing targeting rules from JSON");

                if (string.IsNullOrWhiteSpace(rulesJson))
                {
                    LogWarning($"⚠️ Targeting rules JSON is empty or null");
                    return null;
                }

                // Deserialize JSON to DTO
                var rules = JsonSerializationHelper.DeserializeTargetingRules(rulesJson);

                if (rules == null)
                {
                    LogWarning($"⚠️ Failed to deserialize targeting rules");
                    return null;
                }

                // Validate rules
                var validationErrors = await ValidateRulesAsync(rules);

                if (validationErrors.Any())
                {
                    LogWarning($"⚠️ Targeting rules validation failed: {string.Join(", ", validationErrors)}");
                    return null;
                }

                LogInfo($"✅ Successfully parsed and validated targeting rules");
                return rules;

            }, "ParseAndValidateRulesAsync");
        }

        /// <summary>
        /// Validate targeting rules DTO
        /// Requirements: 4.1
        /// </summary>
        public async Task<List<string>> ValidateRulesAsync(TargetingRulesDto? rules)
        {
            return await ExecuteAsync(async () =>
            {
                var errors = new List<string>();

                if (rules == null)
                {
                    errors.Add("Targeting rules cannot be null");
                    return errors;
                }

                // Validate Pages list
                if (rules.Pages == null || rules.Pages.Count == 0)
                {
                    errors.Add("Pages list is required and must not be empty");
                }
                else
                {
                    // Check for empty page names
                    var emptyPages = rules.Pages.Where(p => string.IsNullOrWhiteSpace(p)).ToList();
                    if (emptyPages.Any())
                    {
                        errors.Add("Page names cannot be empty or whitespace");
                    }
                }

                // Validate Frequency
                if (string.IsNullOrWhiteSpace(rules.Frequency))
                {
                    errors.Add("Frequency is required");
                }
                else if (!ValidFrequencies.Contains(rules.Frequency.ToLower()))
                {
                    errors.Add($"Frequency must be one of: {string.Join(", ", ValidFrequencies)}");
                }

                // Validate DelayMs
                if (rules.DelayMs < 0)
                {
                    errors.Add("DelayMs must be non-negative");
                }

                // Validate AutoDismissMs
                if (rules.AutoDismissMs < 0)
                {
                    errors.Add("AutoDismissMs must be non-negative");
                }

                LogInfo($"✅ Validation completed with {errors.Count} errors");
                return errors;

            }, "ValidateRulesAsync");
        }

        /// <summary>
        /// Check if campaign should be displayed on a given page
        /// Requirements: 4.2
        /// </summary>
        public async Task<bool> ShouldDisplayOnPageAsync(TargetingRulesDto? rules, string currentPage)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo($"🔍 Checking if campaign should display on page: {currentPage}");

                if (rules == null || rules.Pages == null || rules.Pages.Count == 0)
                {
                    LogWarning($"⚠️ No targeting rules or pages defined");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(currentPage))
                {
                    LogWarning($"⚠️ Current page is empty or null");
                    return false;
                }

                // Check if current page is in the targeted pages list (case-insensitive)
                var shouldDisplay = rules.Pages.Any(p =>
                    p.Equals(currentPage, StringComparison.OrdinalIgnoreCase));

                LogInfo($"✅ Page targeting check: {(shouldDisplay ? "MATCH" : "NO MATCH")}");
                return shouldDisplay;

            }, "ShouldDisplayOnPageAsync");
        }

        /// <summary>
        /// Check if campaign should be displayed based on frequency rules
        /// Requirements: 4.3, 4.4, 4.5
        /// </summary>
        public async Task<bool> ShouldDisplayBasedOnFrequencyAsync(
            TargetingRulesDto? rules,
            string sessionId,
            string currentPage,
            List<string> impressionHistory)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo($"🔍 Checking frequency rules - Frequency: {rules?.Frequency}, Session: {sessionId}, Page: {currentPage}");

                if (rules == null || string.IsNullOrWhiteSpace(rules.Frequency))
                {
                    LogWarning($"⚠️ No frequency rules defined");
                    return false;
                }

                if (impressionHistory == null)
                {
                    impressionHistory = new List<string>();
                }

                var frequency = rules.Frequency.ToLower();

                // Requirement 4.5: always - show every time
                if (frequency == "always")
                {
                    LogInfo($"✅ Frequency 'always' - will display");
                    return true;
                }

                // Requirement 4.3: once-per-session - show only once per session
                if (frequency == "once-per-session")
                {
                    var alreadyShownInSession = impressionHistory.Any();
                    LogInfo($"✅ Frequency 'once-per-session' - Already shown: {alreadyShownInSession}");
                    return !alreadyShownInSession;
                }

                // Requirement 4.4: once-per-page - show only once per page per session
                if (frequency == "once-per-page")
                {
                    var alreadyShownOnPage = impressionHistory.Any(p =>
                        p.Equals(currentPage, StringComparison.OrdinalIgnoreCase));
                    LogInfo($"✅ Frequency 'once-per-page' - Already shown on page: {alreadyShownOnPage}");
                    return !alreadyShownOnPage;
                }

                LogWarning($"⚠️ Unknown frequency: {frequency}");
                return false;

            }, "ShouldDisplayBasedOnFrequencyAsync");
        }

        /// <summary>
        /// Get delay before showing campaign
        /// Requirements: 4.4
        /// </summary>
        public async Task<int> GetDisplayDelayAsync(TargetingRulesDto? rules)
        {
            return await ExecuteAsync(async () =>
            {
                if (rules == null)
                {
                    LogInfo($"ℹ️ No targeting rules - using default delay of 0ms");
                    return 0;
                }

                LogInfo($"ℹ️ Display delay: {rules.DelayMs}ms");
                return rules.DelayMs;

            }, "GetDisplayDelayAsync");
        }

        /// <summary>
        /// Get auto-dismiss delay for campaign
        /// Requirements: 4.5
        /// </summary>
        public async Task<int> GetAutoDismissDelayAsync(TargetingRulesDto? rules)
        {
            return await ExecuteAsync(async () =>
            {
                if (rules == null)
                {
                    LogInfo($"ℹ️ No targeting rules - using default auto-dismiss of 0ms (no auto-dismiss)");
                    return 0;
                }

                LogInfo($"ℹ️ Auto-dismiss delay: {rules.AutoDismissMs}ms");
                return rules.AutoDismissMs;

            }, "GetAutoDismissDelayAsync");
        }

        /// <summary>
        /// Evaluate all targeting rules to determine if campaign should be shown
        /// Requirements: 4.1, 4.2, 4.3, 4.4, 4.5
        /// </summary>
        public async Task<bool> EvaluateTargetingRulesAsync(
            TargetingRulesDto? rules,
            string sessionId,
            string currentPage,
            List<string> impressionHistory)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo($"🔍 Evaluating all targeting rules - Session: {sessionId}, Page: {currentPage}");

                if (rules == null)
                {
                    LogWarning($"⚠️ No targeting rules defined");
                    return false;
                }

                // Validate rules first
                var validationErrors = await ValidateRulesAsync(rules);
                if (validationErrors.Any())
                {
                    LogWarning($"⚠️ Targeting rules validation failed: {string.Join(", ", validationErrors)}");
                    return false;
                }

                // Check page targeting (Requirement: 4.2)
                var pageMatches = await ShouldDisplayOnPageAsync(rules, currentPage);
                if (!pageMatches)
                {
                    LogInfo($"❌ Page targeting failed");
                    return false;
                }

                // Check frequency rules (Requirement: 4.3, 4.4, 4.5)
                var frequencyMatches = await ShouldDisplayBasedOnFrequencyAsync(
                    rules, sessionId, currentPage, impressionHistory);
                if (!frequencyMatches)
                {
                    LogInfo($"❌ Frequency rules failed");
                    return false;
                }

                LogInfo($"✅ All targeting rules passed - campaign should be displayed");
                return true;

            }, "EvaluateTargetingRulesAsync");
        }
    }
}
