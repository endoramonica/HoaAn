using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Represents the complete collection of ritual patterns loaded from the manifest.
    /// This class holds all ritual definitions used for sequential pattern matching.
    /// </summary>
    public class RitualManifest
    {
        /// <summary>
        /// Collection of all ritual patterns available in the system
        /// </summary>
        public List<RitualDto> Rituals { get; set; } = new();

        /// <summary>
        /// Gets a ritual by its ID
        /// </summary>
        /// <param name="ritualId">The ID of the ritual to retrieve</param>
        /// <returns>The ritual DTO if found, null otherwise</returns>
        public RitualDto? GetRitualById(string ritualId)
        {
            return Rituals.FirstOrDefault(r => r.Id == ritualId);
        }

        /// <summary>
        /// Gets all active rituals (those with confidence threshold > 0)
        /// </summary>
        /// <returns>List of active ritual DTOs</returns>
        public List<RitualDto> GetActiveRituals()
        {
            return Rituals.Where(r => r.ConfidenceThreshold > 0).ToList();
        }

        /// <summary>
        /// Validates that all required items in all rituals exist in the provided catalog
        /// </summary>
        /// <param name="productCatalog">Dictionary of available product IDs</param>
        /// <returns>Validation result with any missing items</returns>
        public ManifestValidationResult ValidateAgainstCatalog(Dictionary<Guid, string> productCatalog)
        {
            var result = new ManifestValidationResult { IsValid = true };

            foreach (var ritual in Rituals)
            {
                foreach (var requiredItem in ritual.RequiredItems)
                {
                    foreach (var productId in requiredItem.ProductIds)
                    {
                        if (!productCatalog.ContainsKey(productId))
                        {
                            result.IsValid = false;
                            result.MissingItems.Add(new MissingItemInfo
                            {
                                RitualId = ritual.Id,
                                RitualName = ritual.Name,
                                ProductId = productId,
                                CategoryId = requiredItem.CategoryId
                            });
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all rituals that match a given action type
        /// </summary>
        /// <param name="actionType">The action type to search for</param>
        /// <returns>List of rituals containing this action type</returns>
        public List<RitualDto> GetRitualsByActionType(string actionType)
        {
            return Rituals
                .Where(r => r.ActionSequencePattern.Any(a => a.Type == actionType))
                .ToList();
        }

        /// <summary>
        /// Gets the total number of rituals in the manifest
        /// </summary>
        public int RitualCount => Rituals.Count;

        /// <summary>
        /// Checks if the manifest is empty
        /// </summary>
        public bool IsEmpty => Rituals.Count == 0;
    }

    /// <summary>
    /// Result of validating a ritual manifest against a product catalog
    /// </summary>
    public class ManifestValidationResult
    {
        /// <summary>
        /// Whether the manifest is valid (all required items exist in catalog)
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// List of items that are required by rituals but missing from the catalog
        /// </summary>
        public List<MissingItemInfo> MissingItems { get; set; } = new();

        /// <summary>
        /// Gets a summary message of the validation result
        /// </summary>
        public string GetSummary()
        {
            if (IsValid)
                return "Manifest validation passed: all required items exist in catalog.";

            return $"Manifest validation failed: {MissingItems.Count} missing items found.";
        }
    }

    /// <summary>
    /// Information about a missing item in the catalog
    /// </summary>
    public class MissingItemInfo
    {
        /// <summary>
        /// ID of the ritual that requires this item
        /// </summary>
        public string RitualId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the ritual
        /// </summary>
        public string RitualName { get; set; } = string.Empty;

        /// <summary>
        /// ID of the missing product
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Category ID of the missing product
        /// </summary>
        public Guid CategoryId { get; set; }
    }
}
