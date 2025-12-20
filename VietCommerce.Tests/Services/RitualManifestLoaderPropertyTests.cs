using Bogus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Ritual Manifest Loading
    /// Validates: Requirements 7.1, 7.2, 7.3
    /// </summary>
    public class RitualManifestLoaderPropertyTests
    {
        private readonly ILogger<RitualManifestLoader> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RitualManifestLoaderPropertyTests()
        {
            _logger = new LoggerFactory().CreateLogger<RitualManifestLoader>();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Property 24: Ritual Manifest Loading
        /// **Feature: sequential-ritual-recommendation, Property 24: Ritual Manifest Loading**
        /// **Validates: Requirements 7.1**
        /// 
        /// For any valid manifest JSON file, the loader SHALL successfully load it and 
        /// return a RitualManifest object with all rituals preserved.
        /// </summary>
        [Fact]
        public async Task Property_24_RitualManifestLoading()
        {
            // Arrange
            var loader = new RitualManifestLoader(_logger);
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Generate random manifest data
                var faker = new Faker<RitualDto>();
                faker
                    .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
                    .RuleFor(x => x.Name, f => f.Lorem.Word())
                    .RuleFor(x => x.ActionSequencePattern, f => GenerateActionTypeList(f))
                    .RuleFor(x => x.RequiredItems, f => GenerateRequiredItemsList(f))
                    .RuleFor(x => x.ConfidenceThreshold, f => f.Random.Decimal(0.5m, 1m))
                    .RuleFor(x => x.CulturalSignificance, f => f.Lorem.Sentence())
                    .RuleFor(x => x.Sources, f => f.Make(f.Random.Int(1, 3), () => f.Internet.Url()));

                // Act & Assert - Run 100 iterations
                for (int i = 0; i < 100; i++)
                {
                    var rituals = new Faker().Make(new Faker().Random.Int(1, 5), () => faker.Generate()).ToList();
                    var manifest = new RitualManifest { Rituals = rituals };

                    // Write manifest to file
                    var manifestPath = Path.Combine(tempDir, $"manifest-{i}.json");
                    var json = JsonSerializer.Serialize(manifest, _jsonOptions);
                    await File.WriteAllTextAsync(manifestPath, json);

                    // Load manifest
                    var loadedManifest = await loader.LoadManifestAsync(manifestPath);

                    // Assert - Manifest should be loaded correctly
                    Assert.NotNull(loadedManifest);
                    Assert.Equal(manifest.RitualCount, loadedManifest.RitualCount);
                    Assert.Equal(manifest.Rituals.Count, loadedManifest.Rituals.Count);

                    // Verify each ritual is preserved
                    for (int j = 0; j < manifest.Rituals.Count; j++)
                    {
                        Assert.Equal(manifest.Rituals[j].Id, loadedManifest.Rituals[j].Id);
                        Assert.Equal(manifest.Rituals[j].Name, loadedManifest.Rituals[j].Name);
                        Assert.Equal(manifest.Rituals[j].ConfidenceThreshold, loadedManifest.Rituals[j].ConfidenceThreshold);
                    }
                }
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Property 25: Action Sequence Storage in Manifest
        /// **Feature: sequential-ritual-recommendation, Property 25: Action Sequence Storage in Manifest**
        /// **Validates: Requirements 7.2**
        /// 
        /// For any ritual with an action sequence pattern, the manifest SHALL preserve 
        /// all action types and their properties when loaded.
        /// </summary>
        [Fact]
        public async Task Property_25_ActionSequenceStorageInManifest()
        {
            // Arrange
            var loader = new RitualManifestLoader(_logger);
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Generate random action sequences
                var faker = new Faker<ActionTypeDto>();
                faker
                    .RuleFor(x => x.Type, f => f.PickRandom("ViewProduct", "AddToCart", "BrowseCategory"))
                    .RuleFor(x => x.ProductCategoryId, f => f.Random.Bool() ? Guid.NewGuid() : null)
                    .RuleFor(x => x.Metadata, f => f.Random.Bool() ? new Dictionary<string, object> { { "key", "value" } } : null);

                // Act & Assert - Run 100 iterations
                for (int i = 0; i < 100; i++)
                {
                    var actionSequence = new Faker().Make(new Faker().Random.Int(1, 5), () => faker.Generate()).ToList();
                    var ritual = new RitualDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Test Ritual",
                        ActionSequencePattern = actionSequence,
                        RequiredItems = new List<RitualRequiredItemDto>(),
                        ConfidenceThreshold = 0.7m,
                        CulturalSignificance = "Test",
                        Sources = new List<string>()
                    };

                    var manifest = new RitualManifest { Rituals = new List<RitualDto> { ritual } };

                    // Write and load manifest
                    var manifestPath = Path.Combine(tempDir, $"manifest-{i}.json");
                    var json = JsonSerializer.Serialize(manifest, _jsonOptions);
                    await File.WriteAllTextAsync(manifestPath, json);

                    var loadedManifest = await loader.LoadManifestAsync(manifestPath);

                    // Assert - Action sequence should be preserved
                    Assert.NotNull(loadedManifest.Rituals[0].ActionSequencePattern);
                    Assert.Equal(actionSequence.Count, loadedManifest.Rituals[0].ActionSequencePattern.Count);

                    for (int j = 0; j < actionSequence.Count; j++)
                    {
                        Assert.Equal(actionSequence[j].Type, loadedManifest.Rituals[0].ActionSequencePattern[j].Type);
                        Assert.Equal(actionSequence[j].ProductCategoryId, loadedManifest.Rituals[0].ActionSequencePattern[j].ProductCategoryId);
                    }
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Property 26: Required Items Specification
        /// **Feature: sequential-ritual-recommendation, Property 26: Required Items Specification**
        /// **Validates: Requirements 7.3**
        /// 
        /// For any ritual with required items, the manifest SHALL preserve all required 
        /// items and their product IDs when loaded.
        /// </summary>
        [Fact]
        public async Task Property_26_RequiredItemsSpecification()
        {
            // Arrange
            var loader = new RitualManifestLoader(_logger);
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Generate random required items
                var faker = new Faker<RitualRequiredItemDto>();
                faker
                    .RuleFor(x => x.CategoryId, f => Guid.NewGuid())
                    .RuleFor(x => x.ProductIds, f => f.Make(f.Random.Int(1, 5), () => Guid.NewGuid()).ToList());

                // Act & Assert - Run 100 iterations
                for (int i = 0; i < 100; i++)
                {
                    var requiredItems = new Faker().Make(new Faker().Random.Int(1, 3), () => faker.Generate()).ToList();
                    var ritual = new RitualDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Test Ritual",
                        ActionSequencePattern = new List<ActionTypeDto>(),
                        RequiredItems = requiredItems,
                        ConfidenceThreshold = 0.7m,
                        CulturalSignificance = "Test",
                        Sources = new List<string>()
                    };

                    var manifest = new RitualManifest { Rituals = new List<RitualDto> { ritual } };

                    // Write and load manifest
                    var manifestPath = Path.Combine(tempDir, $"manifest-{i}.json");
                    var json = JsonSerializer.Serialize(manifest, _jsonOptions);
                    await File.WriteAllTextAsync(manifestPath, json);

                    var loadedManifest = await loader.LoadManifestAsync(manifestPath);

                    // Assert - Required items should be preserved
                    Assert.NotNull(loadedManifest.Rituals[0].RequiredItems);
                    Assert.Equal(requiredItems.Count, loadedManifest.Rituals[0].RequiredItems.Count);

                    for (int j = 0; j < requiredItems.Count; j++)
                    {
                        Assert.Equal(requiredItems[j].CategoryId, loadedManifest.Rituals[0].RequiredItems[j].CategoryId);
                        Assert.Equal(requiredItems[j].ProductIds.Count, loadedManifest.Rituals[0].RequiredItems[j].ProductIds.Count);

                        for (int k = 0; k < requiredItems[j].ProductIds.Count; k++)
                        {
                            Assert.Equal(requiredItems[j].ProductIds[k], loadedManifest.Rituals[0].RequiredItems[j].ProductIds[k]);
                        }
                    }
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// Property 27: Manifest Validation Against Catalog
        /// **Feature: sequential-ritual-recommendation, Property 27: Manifest Validation Against Catalog**
        /// **Validates: Requirements 7.3**
        /// 
        /// For any manifest and product catalog, the validation SHALL correctly identify 
        /// missing items that are required by rituals but not in the catalog.
        /// </summary>
        [Fact]
        public void Property_27_ManifestValidationAgainstCatalog()
        {
            // Arrange
            var loader = new RitualManifestLoader(_logger);

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                // Create a catalog with some products
                var catalogProducts = new Dictionary<Guid, string>();
                var productIds = new List<Guid>();
                for (int j = 0; j < 10; j++)
                {
                    var productId = Guid.NewGuid();
                    catalogProducts[productId] = $"Product-{j}";
                    productIds.Add(productId);
                }

                // Create a ritual with some products in catalog and some not
                var requiredProductIds = new List<Guid>();
                requiredProductIds.AddRange(productIds.Take(5)); // 5 products in catalog
                requiredProductIds.AddRange(new[] { Guid.NewGuid(), Guid.NewGuid() }); // 2 products not in catalog

                var ritual = new RitualDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Ritual",
                    ActionSequencePattern = new List<ActionTypeDto>(),
                    RequiredItems = new List<RitualRequiredItemDto>
                    {
                        new RitualRequiredItemDto
                        {
                            CategoryId = Guid.NewGuid(),
                            ProductIds = requiredProductIds
                        }
                    },
                    ConfidenceThreshold = 0.7m,
                    CulturalSignificance = "Test",
                    Sources = new List<string>()
                };

                var manifest = new RitualManifest { Rituals = new List<RitualDto> { ritual } };

                // Validate
                var result = loader.ValidateManifest(manifest, catalogProducts);

                // Assert - Should find 2 missing items
                Assert.False(result.IsValid);
                Assert.Equal(2, result.MissingItems.Count);
                Assert.All(result.MissingItems, item => Assert.Equal(ritual.Id, item.RitualId));
            }
        }

        // Helper methods

        private List<ActionTypeDto> GenerateActionTypeList(Faker faker)
        {
            return faker.Make(faker.Random.Int(1, 5), () => new ActionTypeDto
            {
                Type = faker.PickRandom("ViewProduct", "AddToCart", "BrowseCategory"),
                ProductCategoryId = faker.Random.Bool() ? Guid.NewGuid() : null,
                Metadata = null
            }).ToList();
        }

        private List<RitualRequiredItemDto> GenerateRequiredItemsList(Faker faker)
        {
            return faker.Make(faker.Random.Int(1, 3), () => new RitualRequiredItemDto
            {
                CategoryId = Guid.NewGuid(),
                ProductIds = faker.Make(faker.Random.Int(1, 3), () => Guid.NewGuid()).ToList()
            }).ToList();
        }
    }
}
