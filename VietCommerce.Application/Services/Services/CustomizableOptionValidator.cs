using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Service for validating customizable options in package products.
/// Validates JSON structure compliance, required fields, and quantity constraints.
/// Validates: Requirements 1.2, 1.3, 7.2
/// </summary>
public class CustomizableOptionValidator : BaseService
{
    private readonly IValidator<CustomizableOptionDto> _optionValidator;

    public CustomizableOptionValidator(
        ILogger<CustomizableOptionValidator> logger,
        IValidator<CustomizableOptionDto> optionValidator,
        ICacheService? cacheService = null)
        : base(logger, cacheService)
    {
        _optionValidator = optionValidator ?? throw new ArgumentNullException(nameof(optionValidator));
    }

    /// <summary>
    /// Validates a list of customizable options.
    /// Checks JSON structure compliance, required fields, and quantity constraints.
    /// 
    /// Validates:
    /// - All required fields are present (id, name, unitPrice, minQuantity)
    /// - All fields have valid values
    /// - Quantity constraints: minQuantity ≤ maxQuantity (if maxQuantity exists)
    /// - UnitPrice > 0
    /// - MinQuantity > 0
    /// 
    /// Requirements: 1.2, 1.3, 7.2
    /// </summary>
    /// <param name="options">List of customizable options to validate</param>
    /// <returns>ValidationResult with IsValid flag and error messages</returns>
    public async Task<ValidationResult> ValidateCustomizableOptionsAsync(List<CustomizableOptionDto> options)
    {
        return await ExecuteAsync(async () =>
        {
            // Validate input
            if (options == null)
            {
                LogWarning("❌ Customizable options list is null");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Customizable options list cannot be null" }
                };
            }

            if (options.Count == 0)
            {
                LogWarning("❌ Customizable options list is empty");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Customizable options list cannot be empty" }
                };
            }

            // Validate each option
            var allErrors = new List<string>();
            var optionIndex = 0;

            foreach (var option in options)
            {
                // Validate individual option using FluentValidation
                var validationResult = await _optionValidator.ValidateAsync(option);

                if (!validationResult.IsValid)
                {
                    // Collect errors with option index for clarity
                    foreach (var error in validationResult.Errors)
                    {
                        allErrors.Add($"Option {optionIndex} - {error.PropertyName}: {error.ErrorMessage}");
                    }
                }

                optionIndex++;
            }

            // Check for duplicate option IDs
            var duplicateIds = options
                .GroupBy(o => o.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                allErrors.Add($"Duplicate option IDs found: {string.Join(", ", duplicateIds)}");
            }

            // Return result
            if (allErrors.Any())
            {
                LogWarning($"❌ Customizable options validation failed with {allErrors.Count} error(s)");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = allErrors.ToArray()
                };
            }

            LogInfo("✅ Customizable options validation passed");
            return new ValidationResult
            {
                IsValid = true,
                Errors = Array.Empty<string>()
            };
        },
        "ValidateCustomizableOptions");
    }
}

/// <summary>
/// Result of validation operation.
/// Contains IsValid flag and error messages if validation fails.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates whether validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Array of error messages if validation failed.
    /// Empty array if validation passed.
    /// </summary>
    public string[] Errors { get; set; } = Array.Empty<string>();
}
