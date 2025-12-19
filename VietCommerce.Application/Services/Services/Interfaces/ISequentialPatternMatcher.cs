using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for sequential pattern matching using PrefixSpan-inspired algorithm
    /// </summary>
    public interface ISequentialPatternMatcher
    {
        /// <summary>
        /// Initializes the pattern matcher with a ritual manifest
        /// </summary>
        /// <param name="manifest">The ritual manifest containing all patterns to match against</param>
        void Initialize(RitualManifest manifest);

        /// <summary>
        /// Matches an action sequence against all ritual patterns in the manifest
        /// </summary>
        /// <param name="actionSequence">The sequence of user actions to analyze</param>
        /// <returns>MatchResult containing the best matching ritual and confidence score</returns>
        MatchResultDto MatchPattern(List<ActionDto> actionSequence);

        /// <summary>
        /// Gets a ritual by its ID from the loaded manifest
        /// </summary>
        /// <param name="ritualId">The ID of the ritual to retrieve</param>
        /// <returns>The ritual DTO if found, null otherwise</returns>
        RitualDto? GetRitualById(string ritualId);

        /// <summary>
        /// Gets all active rituals from the loaded manifest
        /// </summary>
        /// <returns>List of active ritual DTOs</returns>
        List<RitualDto> GetActiveRituals();

        /// <summary>
        /// Checks if the pattern matcher has been initialized with a manifest
        /// </summary>
        /// <returns>True if initialized, false otherwise</returns>
        bool IsInitialized();
    }
}
