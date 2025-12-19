using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Implements sequential pattern matching using a PrefixSpan-inspired algorithm
    /// for detecting Vietnamese cultural rituals from user action sequences
    /// </summary>
    public class SequentialPatternMatcher : ISequentialPatternMatcher
    {
        private RitualManifest? _manifest;
        private bool _initialized = false;

        /// <summary>
        /// Initializes the pattern matcher with a ritual manifest
        /// </summary>
        /// <param name="manifest">The ritual manifest containing all patterns to match against</param>
        public void Initialize(RitualManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _initialized = true;
        }

        /// <summary>
        /// Matches an action sequence against all ritual patterns in the manifest
        /// Uses a PrefixSpan-inspired algorithm to find the best matching ritual
        /// </summary>
        /// <param name="actionSequence">The sequence of user actions to analyze</param>
        /// <returns>MatchResult containing the best matching ritual and confidence score</returns>
        public MatchResultDto MatchPattern(List<ActionDto> actionSequence)
        {
            if (!_initialized || _manifest == null)
            {
                return new MatchResultDto { Matched = false };
            }

            if (actionSequence == null || actionSequence.Count == 0)
            {
                return new MatchResultDto { Matched = false };
            }

            var bestMatch = new MatchResultDto { Matched = false };
            decimal bestConfidence = 0;

            // Iterate through all active rituals in the manifest
            foreach (var ritual in _manifest.GetActiveRituals())
            {
                var matchResult = MatchRitualPattern(ritual, actionSequence);

                // Update best match if this ritual has higher confidence
                if (matchResult.Matched && matchResult.ConfidenceScore > bestConfidence)
                {
                    bestConfidence = matchResult.ConfidenceScore;
                    bestMatch = matchResult;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Matches a specific ritual pattern against an action sequence
        /// </summary>
        /// <param name="ritual">The ritual pattern to match</param>
        /// <param name="actionSequence">The action sequence to analyze</param>
        /// <returns>MatchResult for this specific ritual</returns>
        private MatchResultDto MatchRitualPattern(RitualDto ritual, List<ActionDto> actionSequence)
        {
            var result = new MatchResultDto
            {
                RitualId = ritual.Id,
                RitualName = ritual.Name,
                Matched = false,
                ConfidenceScore = 0
            };

            if (ritual.ActionSequencePattern == null || ritual.ActionSequencePattern.Count == 0)
            {
                return result;
            }

            // Extract action types from the user's action sequence
            var userActionTypes = actionSequence.Select(a => a.Type).ToList();

            // Find matching subsequence using PrefixSpan-inspired approach
            var matchIndices = FindMatchingSubsequence(
                ritual.ActionSequencePattern.Select(a => a.Type).ToList(),
                userActionTypes
            );

            if (matchIndices.Count == 0)
            {
                return result;
            }

            // Calculate confidence score based on match quality
            var confidenceScore = CalculateConfidenceScore(
                ritual.ActionSequencePattern.Count,
                matchIndices.Count,
                actionSequence.Count
            );

            // Check if confidence meets the ritual's threshold
            if (confidenceScore < ritual.ConfidenceThreshold)
            {
                return result;
            }

            // Build the matched actions list
            var matchedActions = matchIndices
                .Select(idx => actionSequence[idx])
                .ToList();

            result.Matched = true;
            result.ConfidenceScore = confidenceScore;
            result.MatchedActions = matchedActions;
            result.MatchingMetadata = new MatchingMetadataDto
            {
                MatchedSequenceLength = matchIndices.Count,
                TotalSequenceLength = actionSequence.Count,
                MatchedActionIndices = matchIndices
            };

            return result;
        }

        /// <summary>
        /// Finds a matching subsequence using PrefixSpan-inspired algorithm
        /// This is a simplified version that finds the longest common subsequence
        /// </summary>
        /// <param name="pattern">The pattern to match (ritual action types)</param>
        /// <param name="sequence">The sequence to search in (user action types)</param>
        /// <returns>List of indices in the sequence that match the pattern</returns>
        private List<int> FindMatchingSubsequence(List<string> pattern, List<string> sequence)
        {
            if (pattern.Count == 0 || sequence.Count == 0)
            {
                return new List<int>();
            }

            // Use dynamic programming to find the longest common subsequence
            var matchIndices = new List<int>();
            int patternIdx = 0;

            for (int seqIdx = 0; seqIdx < sequence.Count && patternIdx < pattern.Count; seqIdx++)
            {
                if (sequence[seqIdx] == pattern[patternIdx])
                {
                    matchIndices.Add(seqIdx);
                    patternIdx++;
                }
            }

            // Only return if we matched the entire pattern
            if (patternIdx == pattern.Count)
            {
                return matchIndices;
            }

            return new List<int>();
        }

        /// <summary>
        /// Calculates a confidence score based on pattern match quality
        /// </summary>
        /// <param name="patternLength">Length of the ritual pattern</param>
        /// <param name="matchedLength">Number of actions that matched</param>
        /// <param name="sequenceLength">Total length of the action sequence</param>
        /// <returns>Confidence score between 0 and 1</returns>
        private decimal CalculateConfidenceScore(int patternLength, int matchedLength, int sequenceLength)
        {
            if (patternLength == 0 || matchedLength == 0)
            {
                return 0;
            }

            // Confidence is based on:
            // 1. How much of the pattern was matched (matchedLength / patternLength)
            // 2. How concentrated the matches are in the sequence (inverse of sequence length)
            
            decimal patternCoverage = (decimal)matchedLength / patternLength;
            decimal sequenceDensity = (decimal)matchedLength / sequenceLength;

            // Weight pattern coverage more heavily (70%) than sequence density (30%)
            decimal confidence = (patternCoverage * 0.7m) + (sequenceDensity * 0.3m);

            // Ensure confidence is between 0 and 1
            return Math.Min(1, Math.Max(0, confidence));
        }

        /// <summary>
        /// Gets a ritual by its ID from the loaded manifest
        /// </summary>
        /// <param name="ritualId">The ID of the ritual to retrieve</param>
        /// <returns>The ritual DTO if found, null otherwise</returns>
        public RitualDto? GetRitualById(string ritualId)
        {
            if (!_initialized || _manifest == null)
            {
                return null;
            }

            return _manifest.GetRitualById(ritualId);
        }

        /// <summary>
        /// Gets all active rituals from the loaded manifest
        /// </summary>
        /// <returns>List of active ritual DTOs</returns>
        public List<RitualDto> GetActiveRituals()
        {
            if (!_initialized || _manifest == null)
            {
                return new List<RitualDto>();
            }

            return _manifest.GetActiveRituals();
        }

        /// <summary>
        /// Checks if the pattern matcher has been initialized with a manifest
        /// </summary>
        /// <returns>True if initialized, false otherwise</returns>
        public bool IsInitialized()
        {
            return _initialized && _manifest != null;
        }
    }
}
