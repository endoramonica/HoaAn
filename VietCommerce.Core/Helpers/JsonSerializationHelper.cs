using System.Text.Json;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Core.Helpers
{
    /// <summary>
    /// Helper class for JSON serialization and deserialization of customizable options and customizations.
    /// Provides graceful error handling for JSON operations.
    /// </summary>
    public static class JsonSerializationHelper
    {
        /// <summary>
        /// Serializes a list of CustomizableOptionDto to JSON string.
        /// </summary>
        /// <param name="options">List of customizable options to serialize</param>
        /// <returns>JSON string representation of the options, or null if serialization fails</returns>
        /// <remarks>
        /// Handles serialization errors gracefully by logging and returning null.
        /// Validates: Requirements 1.4, 7.4
        /// </remarks>
        public static string? SerializeCustomizableOptions(List<CustomizableOptionDto>? options)
        {
            try
            {
                if (options == null || options.Count == 0)
                {
                    return null;
                }

                return JsonSerializer.Serialize(options, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
            }
            catch (JsonException ex)
            {
                // Log serialization error
                System.Diagnostics.Debug.WriteLine($"Error serializing customizable options: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during customizable options serialization: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to a list of CustomizableOptionDto.
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>List of CustomizableOptionDto, or empty list if deserialization fails</returns>
        /// <remarks>
        /// Handles deserialization errors gracefully by logging and returning empty list.
        /// Validates: Requirements 1.4, 7.4
        /// </remarks>
        public static List<CustomizableOptionDto> DeserializeCustomizableOptions(string? json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<CustomizableOptionDto>();
                }

                var options = JsonSerializer.Deserialize<List<CustomizableOptionDto>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                return options ?? new List<CustomizableOptionDto>();
            }
            catch (JsonException ex)
            {
                // Log deserialization error
                System.Diagnostics.Debug.WriteLine($"Error deserializing customizable options: {ex.Message}");
                return new List<CustomizableOptionDto>();
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during customizable options deserialization: {ex.Message}");
                return new List<CustomizableOptionDto>();
            }
        }

        /// <summary>
        /// Serializes a list of product details (strings) to JSON string.
        /// </summary>
        /// <param name="details">List of product details to serialize</param>
        /// <returns>JSON string representation of the details, or null if serialization fails</returns>
        /// <remarks>
        /// Handles serialization errors gracefully by logging and returning null.
        /// Validates: Requirements 1.4, 7.4
        /// </remarks>
        public static string? SerializeDetails(List<string>? details)
        {
            try
            {
                if (details == null || details.Count == 0)
                {
                    return null;
                }

                return JsonSerializer.Serialize(details, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
            }
            catch (JsonException ex)
            {
                // Log serialization error
                System.Diagnostics.Debug.WriteLine($"Error serializing product details: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during product details serialization: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to a list of product details (strings).
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>List of product details, or empty list if deserialization fails</returns>
        /// <remarks>
        /// Handles deserialization errors gracefully by logging and returning empty list.
        /// Validates: Requirements 1.4, 7.4
        /// </remarks>
        public static List<string> DeserializeDetails(string? json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<string>();
                }

                var details = JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                return details ?? new List<string>();
            }
            catch (JsonException ex)
            {
                // Log deserialization error
                System.Diagnostics.Debug.WriteLine($"Error deserializing product details: {ex.Message}");
                return new List<string>();
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during product details deserialization: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Serializes a list of CartItemCustomizationDto to JSON string.
        /// </summary>
        /// <param name="customizations">List of customizations to serialize</param>
        /// <returns>JSON string representation of the customizations, or null if serialization fails</returns>
        /// <remarks>
        /// Handles serialization errors gracefully by logging and returning null.
        /// Validates: Requirements 3.4, 7.4
        /// </remarks>
        public static string? SerializeCustomizations(List<CartItemCustomizationDto>? customizations)
        {
            try
            {
                if (customizations == null || customizations.Count == 0)
                {
                    return null;
                }

                return JsonSerializer.Serialize(customizations, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
            }
            catch (JsonException ex)
            {
                // Log serialization error
                System.Diagnostics.Debug.WriteLine($"Error serializing customizations: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during customizations serialization: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to a list of CartItemCustomizationDto.
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>List of CartItemCustomizationDto, or empty list if deserialization fails</returns>
        /// <remarks>
        /// Handles deserialization errors gracefully by logging and returning empty list.
        /// Validates: Requirements 3.4, 7.4
        /// </remarks>
        public static List<CartItemCustomizationDto> DeserializeCustomizations(string? json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<CartItemCustomizationDto>();
                }

                var customizations = JsonSerializer.Deserialize<List<CartItemCustomizationDto>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                return customizations ?? new List<CartItemCustomizationDto>();
            }
            catch (JsonException ex)
            {
                // Log deserialization error
                System.Diagnostics.Debug.WriteLine($"Error deserializing customizations: {ex.Message}");
                return new List<CartItemCustomizationDto>();
            }
            catch (Exception ex)
            {
                // Log unexpected error
                System.Diagnostics.Debug.WriteLine($"Unexpected error during customizations deserialization: {ex.Message}");
                return new List<CartItemCustomizationDto>();
            }
        }
    }
}
