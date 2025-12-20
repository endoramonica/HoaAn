using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Represents the complete collection of ritual patterns loaded from the manifest.
    /// This class holds all ritual definitions used for sequential pattern matching.
    /// Includes optimized indexing for efficient pattern lookup by action types.
    /// </summary>
    public class RitualManifest
    {
        /// <summary>
        /// Collection of all ritual patterns available in the system
        /// </summary>
        public List<RitualDto> Rituals { get; set; } = new();

        /// <summary>
        /// Index mapping action types to rituals that contain them
        /// Enables O(1) lookup of rituals by action type instead of O(n)
        /// </summary>
        private Dictionary<string, List<RitualDto>> _actionTypeIndex = new();

        /// <summary>
        /// Index mapping ritual IDs to rituals for O(1) lookup
        /// </summary>
        private Dictionary<string, RitualDto> _ritualIdIndex = new();

        /// <summary>
        /// Flag indicating whether indexes have been built
        /// </summary>
        private bool _indexesBuilt = false;

        /// <summary>
        /// Gets a ritual by its ID using indexed lookup
        /// </summary>
        /// <param name="ritualId">The ID of the ritual to retrieve</param>
        /// <returns>The ritual DTO if found, null otherwise</returns>
        public RitualDto? GetRitualById(string ritualId)
        {
            EnsureIndexesBuilt();
            return _ritualIdIndex.TryGetValue(ritualId, out var ritual) ? ritual : null;
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
        /// Gets all rituals that match a given action type using indexed lookup
        /// Provides O(1) lookup instead of O(n) by scanning all rituals
        /// </summary>
        /// <param name="actionType">The action type to search for</param>
        /// <returns>List of rituals containing this action type</returns>
        public List<RitualDto> GetRitualsByActionType(string actionType)
        {
            EnsureIndexesBuilt();
            return _actionTypeIndex.TryGetValue(actionType, out var rituals) 
                ? new List<RitualDto>(rituals) 
                : new List<RitualDto>();
        }

        /// <summary>
        /// Gets all rituals that start with a given action type
        /// Useful for optimizing pattern matching by filtering candidates early
        /// </summary>
        /// <param name="actionType">The action type to search for</param>
        /// <returns>List of rituals that have this action type as their first action</returns>
        public List<RitualDto> GetRitualsByStartingActionType(string actionType)
        {
            EnsureIndexesBuilt();
            return Rituals
                .Where(r => r.ActionSequencePattern.Count > 0 && r.ActionSequencePattern[0].Type == actionType)
                .ToList();
        }

        /// <summary>
        /// Gets all rituals that contain a specific sequence of action types
        /// Enables efficient filtering of candidate rituals for pattern matching
        /// </summary>
        /// <param name="actionTypes">The sequence of action types to search for</param>
        /// <returns>List of rituals that contain all these action types in order</returns>
        public List<RitualDto> GetRitualsByActionSequence(List<string> actionTypes)
        {
            if (actionTypes == null || actionTypes.Count == 0)
            {
                return new List<RitualDto>();
            }

            EnsureIndexesBuilt();

            // Start with rituals that have the first action type
            var candidates = GetRitualsByStartingActionType(actionTypes[0]);

            // Filter to only those that contain all action types in sequence
            return candidates
                .Where(r => ContainsActionSequence(r, actionTypes))
                .ToList();
        }

        /// <summary>
        /// Checks if a ritual contains a specific sequence of action types
        /// </summary>
        /// <param name="ritual">The ritual to check</param>
        /// <param name="actionTypes">The sequence of action types to find</param>
        /// <returns>True if the ritual contains the sequence, false otherwise</returns>
        private bool ContainsActionSequence(RitualDto ritual, List<string> actionTypes)
        {
            if (actionTypes.Count == 0 || ritual.ActionSequencePattern.Count < actionTypes.Count)
            {
                return false;
            }

            int patternIdx = 0;
            for (int ritualIdx = 0; ritualIdx < ritual.ActionSequencePattern.Count && patternIdx < actionTypes.Count; ritualIdx++)
            {
                if (ritual.ActionSequencePattern[ritualIdx].Type == actionTypes[patternIdx])
                {
                    patternIdx++;
                }
            }

            return patternIdx == actionTypes.Count;
        }

        /// <summary>
        /// Rebuilds the internal indexes for efficient lookup
        /// Should be called after rituals are loaded or modified
        /// </summary>
        public void RebuildIndexes()
        {
            _actionTypeIndex.Clear();
            _ritualIdIndex.Clear();

            foreach (var ritual in Rituals)
            {
                // Build ritual ID index
                _ritualIdIndex[ritual.Id] = ritual;

                // Build action type index
                foreach (var action in ritual.ActionSequencePattern)
                {
                    if (!_actionTypeIndex.ContainsKey(action.Type))
                    {
                        _actionTypeIndex[action.Type] = new List<RitualDto>();
                    }

                    if (!_actionTypeIndex[action.Type].Contains(ritual))
                    {
                        _actionTypeIndex[action.Type].Add(ritual);
                    }
                }
            }

            _indexesBuilt = true;
        }

        /// <summary>
        /// Ensures indexes are built before use
        /// </summary>
        private void EnsureIndexesBuilt()
        {
            if (!_indexesBuilt)
            {
                RebuildIndexes();
            }
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
