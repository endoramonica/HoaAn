namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for loading and managing ritual manifests
    /// </summary>
    public interface IRitualManifestLoader
    {
        /// <summary>
        /// Loads a ritual manifest from a JSON file
        /// </summary>
        /// <param name="manifestPath">Path to the manifest JSON file</param>
        /// <returns>Loaded RitualManifest object</returns>
        /// <exception cref="FileNotFoundException">Thrown if manifest file is not found</exception>
        /// <exception cref="InvalidOperationException">Thrown if manifest JSON is invalid</exception>
        Task<RitualManifest> LoadManifestAsync(string manifestPath);

        /// <summary>
        /// Validates a loaded manifest against a product catalog
        /// </summary>
        /// <param name="manifest">The manifest to validate</param>
        /// <param name="productCatalog">Dictionary of available product IDs</param>
        /// <returns>Validation result</returns>
        ManifestValidationResult ValidateManifest(RitualManifest manifest, Dictionary<Guid, string> productCatalog);

        /// <summary>
        /// Gets the currently loaded manifest
        /// </summary>
        /// <returns>The current manifest, or null if none is loaded</returns>
        RitualManifest? GetCurrentManifest();

        /// <summary>
        /// Reloads the manifest from the specified path
        /// </summary>
        /// <param name="manifestPath">Path to the manifest JSON file</param>
        /// <returns>Reloaded RitualManifest object</returns>
        Task<RitualManifest> ReloadManifestAsync(string manifestPath);
    }
}
