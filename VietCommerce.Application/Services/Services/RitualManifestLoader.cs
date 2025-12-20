using Microsoft.Extensions.Logging;
using System.Text.Json;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service for loading and managing ritual manifests from JSON files.
    /// Handles manifest loading, validation, and caching.
    /// </summary>
    public class RitualManifestLoader : BaseService, IRitualManifestLoader
    {
        private RitualManifest? _currentManifest;
        private readonly JsonSerializerOptions _jsonOptions;

        public RitualManifestLoader(ILogger<RitualManifestLoader> logger, ICacheService? cacheService = null)
            : base(logger, cacheService)
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Loads a ritual manifest from a JSON file
        /// </summary>
        public async Task<RitualManifest> LoadManifestAsync(string manifestPath)
        {
            return await ExecuteAsync(async () =>
            {
                ValidateNotEmpty(manifestPath, nameof(manifestPath));

                if (!File.Exists(manifestPath))
                {
                    throw new FileNotFoundException($"Manifest file not found at path: {manifestPath}");
                }

                LogInfo($"📖 Loading ritual manifest from: {manifestPath}");

                var json = await File.ReadAllTextAsync(manifestPath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new InvalidOperationException("Manifest file is empty");
                }

                var manifest = JsonSerializer.Deserialize<RitualManifest>(json, _jsonOptions);

                if (manifest == null)
                {
                    throw new InvalidOperationException("Failed to deserialize manifest JSON");
                }

                if (manifest.IsEmpty)
                {
                    LogWarning("⚠️ Loaded manifest contains no rituals");
                }
                else
                {
                    LogInfo($"✅ Successfully loaded manifest with {manifest.RitualCount} rituals");
                }

                _currentManifest = manifest;
                return manifest;
            }, "LoadManifestAsync");
        }

        /// <summary>
        /// Validates a loaded manifest against a product catalog
        /// </summary>
        public ManifestValidationResult ValidateManifest(RitualManifest manifest, Dictionary<Guid, string> productCatalog)
        {
            return Execute(() =>
            {
                ValidateNotNull(manifest, nameof(manifest));
                ValidateNotNull(productCatalog, nameof(productCatalog));

                LogInfo($"🔍 Validating manifest with {manifest.RitualCount} rituals against catalog with {productCatalog.Count} products");

                var result = manifest.ValidateAgainstCatalog(productCatalog);

                if (result.IsValid)
                {
                    LogInfo("✅ Manifest validation passed");
                }
                else
                {
                    LogWarning($"⚠️ Manifest validation failed: {result.MissingItems.Count} missing items");
                    foreach (var missing in result.MissingItems)
                    {
                        LogWarning($"   - Ritual '{missing.RitualName}' requires product {missing.ProductId} which is not in catalog");
                    }
                }

                return result;
            }, "ValidateManifest");
        }

        /// <summary>
        /// Gets the currently loaded manifest
        /// </summary>
        public RitualManifest? GetCurrentManifest()
        {
            return _currentManifest;
        }

        /// <summary>
        /// Reloads the manifest from the specified path
        /// </summary>
        public async Task<RitualManifest> ReloadManifestAsync(string manifestPath)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo($"🔄 Reloading ritual manifest from: {manifestPath}");
                _currentManifest = null;
                return await LoadManifestAsync(manifestPath);
            }, "ReloadManifestAsync");
        }
    }
}
