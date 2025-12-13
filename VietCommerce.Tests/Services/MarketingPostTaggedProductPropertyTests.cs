using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Identity;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using AutoMapper;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services.Interfaces.Identities;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for TaggedProduct reflecting live product data
    /// **Feature: admin-marketing-post, Property 27: TaggedProduct reflects live product data**
    /// **Validates: Requirements 16.6**
    /// </summary>
    public class MarketingPostTaggedProductPropertyTests
    {
        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MarketingPostMappingProfile>();
            });
            return config.CreateMapper();
        }

        private Mock<IUnitOfWork> CreateMockUnitOfWork()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMarketingPostRepository = new Mock<IMarketingPostRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            mockUnitOfWork.Setup(u => u.MarketingPosts).Returns(mockMarketingPostRepository.Object);
            mockUnitOfWork.Setup(u => u.Products).Returns(mockProductRepository.Object);

            return mockUnitOfWork;
        }

        private Mock<ICurrentUser> CreateMockCurrentUser()
        {
            var mockCurrentUser = new Mock<ICurrentUser>();
            mockCurrentUser.Setup(u => u.UserId).Returns(Guid.NewGuid());
            mockCurrentUser.Setup(u => u.UserName).Returns("testadmin");
            return mockCurrentUser;
        }

        private Mock<ICacheService> CreateMockCacheService()
        {
            var mockCacheService = new Mock<ICacheService>();
            mockCacheService.DefaultValue = DefaultValue.Mock;
            mockCacheService
                .Setup(c => c.GetAsync<object>(It.IsAny<string>()))
                .ReturnsAsync((object)null!);
            return mockCacheService;
        }

        /// <summary>
        /// Property 27: TaggedProduct reflects live product data
        /// For any post with a productId, retrieving the post should return the latest product information
        /// including current price, discount, and image data.
        /// **Validates: Requirements 16.6**
        /// </summary>
        [Fact]
        public async Task Property_27_TaggedProductReflectsLiveProductData()
        {
            // **Feature: admin-marketing-post, Property 27: TaggedProduct reflects live product data**
            // **Validates: Requirements 16.6**

            var faker = new Faker();
            var mapper = CreateMapper();

            // Run 100 iterations with random data
            for (int i = 0; i < 100; i++)
            {
                var mockUnitOfWork = CreateMockUnitOfWork();
                var mockCurrentUser = CreateMockCurrentUser();
                var mockLogger = new Mock<ILogger<MarketingPostService>>();
                var mockCacheService = CreateMockCacheService();

                var marketingPostService = new MarketingPostService(
                    mockUnitOfWork.Object,
                    mapper,
                    mockCurrentUser.Object,
                    mockLogger.Object,
                    mockCacheService.Object
                );

                var postId = Guid.NewGuid();
                var productId = Guid.NewGuid();

                // Create a marketing post with productId
                var marketingPost = new MarketingPost
                {
                    Id = postId,
                    Title = faker.Commerce.ProductName(),
                    Content = faker.Lorem.Paragraphs(3),
                    ShortDescription = faker.Lorem.Sentence(),
                    ProductId = productId,
                    ProductName = faker.Commerce.ProductName(),
                    Status = MarketingPostStatus.Published,
                    PriorityScore = faker.Random.Int(1, 100),
                    Views = faker.Random.Int(0, 1000),
                    Clicks = faker.Random.Int(0, 500),
                    Shares = faker.Random.Int(0, 100),
                    CreatedAt = DateTime.UtcNow.AddDays(-faker.Random.Int(1, 30)),
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = mockCurrentUser.Object.UserId,
                    IsDeleted = false,
                    IsActive = true
                };

                // Create a product with pricing and image
                var regularPrice = faker.Random.Decimal(10000, 1000000);
                var salePrice = faker.Random.Decimal(regularPrice * 0.5m, regularPrice * 0.9m);
                var hasDiscount = faker.Random.Bool();

                var product = new Product
                {
                    Id = productId,
                    Name = faker.Commerce.ProductName(),
                    Slug = faker.Lorem.Slug(),
                    IsActive = true,
                    Prices = new List<ProductPrice>
                    {
                        new ProductPrice
                        {
                            Id = Guid.NewGuid(),
                            ProductId = productId,
                            Price = regularPrice,
                            PriceType = PriceType.REGULAR,
                            IsActive = true,
                            EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                        }
                    },
                    Images = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            Id = Guid.NewGuid(),
                            ProductId = productId,
                            Url = faker.Image.PicsumUrl(),
                            ThumbnailUrl = faker.Image.PicsumUrl(),
                            IsMain = true,
                            DisplayOrder = 1
                        }
                    }
                };

                // Add sale price if discount is applicable
                if (hasDiscount)
                {
                    product.Prices.Add(new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = salePrice,
                        PriceType = PriceType.SALE,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-5)
                    });
                }

                // Setup repository mocks
                mockUnitOfWork.Setup(u => u.MarketingPosts.GetByIdAsync(postId, false))
                    .ReturnsAsync(marketingPost);

                mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                    .ReturnsAsync(product);

                // Get the post
                var result = await marketingPostService.GetPostByIdAsync(postId);

                // Verify success
                Assert.True(result.Success, $"Iteration {i}: Post retrieval should succeed");
                Assert.NotNull(result.Data);

                // Verify TaggedProduct is populated
                Assert.NotNull(result.Data.TaggedProduct);

                // Verify TaggedProduct contains live product data
                Assert.Equal(productId, result.Data.TaggedProduct.Id);
                Assert.Equal(product.Name, result.Data.TaggedProduct.Name);
                Assert.Equal("VND", result.Data.TaggedProduct.Currency);

                // Verify price is current (live data)
                var expectedPrice = hasDiscount ? salePrice : regularPrice;
                Assert.Equal(expectedPrice, result.Data.TaggedProduct.Price);

                // Verify discount information
                Assert.Equal(hasDiscount, result.Data.TaggedProduct.HasDiscount);

                // Verify thumbnail URL is populated
                Assert.NotEmpty(result.Data.TaggedProduct.ThumbnailUrl);

                // Verify formatted price is populated
                Assert.NotEmpty(result.Data.TaggedProduct.FormattedPrice);
                Assert.Contains("VND", result.Data.TaggedProduct.FormattedPrice);
            }
        }

        /// <summary>
        /// Property 27 Edge Case: Product price changes
        /// For any post with a productId, if the product's price changes, retrieving the post
        /// should return the updated price information.
        /// **Validates: Requirements 16.6**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_ProductPriceChanges()
        {
            // **Feature: admin-marketing-post, Property 27: TaggedProduct reflects live product data (Edge Case)**
            // **Validates: Requirements 16.6**

            var faker = new Faker();
            var mapper = CreateMapper();

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCurrentUser = CreateMockCurrentUser();
            var mockLogger = new Mock<ILogger<MarketingPostService>>();
            var mockCacheService = CreateMockCacheService();

            var marketingPostService = new MarketingPostService(
                mockUnitOfWork.Object,
                mapper,
                mockCurrentUser.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var postId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Create a marketing post
            var marketingPost = new MarketingPost
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test content",
                ProductId = productId,
                Status = MarketingPostStatus.Published,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = mockCurrentUser.Object.UserId,
                IsDeleted = false,
                IsActive = true
            };

            // First retrieval: product with price 100,000
            var product1 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image1.jpg",
                        ThumbnailUrl = "https://example.com/thumb1.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.MarketingPosts.GetByIdAsync(postId, false))
                .ReturnsAsync(marketingPost);

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product1);

            // Get post first time
            var result1 = await marketingPostService.GetPostByIdAsync(postId);
            Assert.Equal(100000, result1.Data.TaggedProduct.Price);

            // Second retrieval: product with updated price 80,000
            var product2 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    },
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 80000,
                        PriceType = PriceType.SALE,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image1.jpg",
                        ThumbnailUrl = "https://example.com/thumb1.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product2);

            // Get post second time
            var result2 = await marketingPostService.GetPostByIdAsync(postId);

            // Verify price has been updated to reflect live data
            Assert.Equal(80000, result2.Data.TaggedProduct.Price);
            Assert.True(result2.Data.TaggedProduct.HasDiscount);
            Assert.True(result2.Data.TaggedProduct.DiscountPercentage > 0);
        }

        /// <summary>
        /// Property 27 Edge Case: Product image changes
        /// For any post with a productId, if the product's image changes, retrieving the post
        /// should return the updated image URL.
        /// **Validates: Requirements 16.6**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_ProductImageChanges()
        {
            // **Feature: admin-marketing-post, Property 27: TaggedProduct reflects live product data (Edge Case)**
            // **Validates: Requirements 16.6**

            var mapper = CreateMapper();

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCurrentUser = CreateMockCurrentUser();
            var mockLogger = new Mock<ILogger<MarketingPostService>>();
            var mockCacheService = CreateMockCacheService();

            var marketingPostService = new MarketingPostService(
                mockUnitOfWork.Object,
                mapper,
                mockCurrentUser.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var postId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Create a marketing post
            var marketingPost = new MarketingPost
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test content",
                ProductId = productId,
                Status = MarketingPostStatus.Published,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = mockCurrentUser.Object.UserId,
                IsDeleted = false,
                IsActive = true
            };

            // First retrieval: product with image1
            var product1 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image1.jpg",
                        ThumbnailUrl = "https://example.com/thumb1.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.MarketingPosts.GetByIdAsync(postId, false))
                .ReturnsAsync(marketingPost);

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product1);

            // Get post first time
            var result1 = await marketingPostService.GetPostByIdAsync(postId);
            var originalThumbnail = result1.Data.TaggedProduct.ThumbnailUrl;
            Assert.Equal("https://example.com/thumb1.jpg", originalThumbnail);

            // Second retrieval: product with updated image
            var product2 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image2.jpg",
                        ThumbnailUrl = "https://example.com/thumb2.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product2);

            // Get post second time
            var result2 = await marketingPostService.GetPostByIdAsync(postId);

            // Verify image has been updated to reflect live data
            Assert.Equal("https://example.com/thumb2.jpg", result2.Data.TaggedProduct.ThumbnailUrl);
            Assert.NotEqual(originalThumbnail, result2.Data.TaggedProduct.ThumbnailUrl);
        }

        /// <summary>
        /// Property 27 Edge Case: Product discount changes
        /// For any post with a productId, if the product's discount changes, retrieving the post
        /// should return the updated discount information.
        /// **Validates: Requirements 16.6**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_ProductDiscountChanges()
        {
            // **Feature: admin-marketing-post, Property 27: TaggedProduct reflects live product data (Edge Case)**
            // **Validates: Requirements 16.6**

            var mapper = CreateMapper();

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCurrentUser = CreateMockCurrentUser();
            var mockLogger = new Mock<ILogger<MarketingPostService>>();
            var mockCacheService = CreateMockCacheService();

            var marketingPostService = new MarketingPostService(
                mockUnitOfWork.Object,
                mapper,
                mockCurrentUser.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var postId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Create a marketing post
            var marketingPost = new MarketingPost
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test content",
                ProductId = productId,
                Status = MarketingPostStatus.Published,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = mockCurrentUser.Object.UserId,
                IsDeleted = false,
                IsActive = true
            };

            // First retrieval: product without discount
            var product1 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image1.jpg",
                        ThumbnailUrl = "https://example.com/thumb1.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.MarketingPosts.GetByIdAsync(postId, false))
                .ReturnsAsync(marketingPost);

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product1);

            // Get post first time
            var result1 = await marketingPostService.GetPostByIdAsync(postId);
            Assert.False(result1.Data.TaggedProduct.HasDiscount);
            Assert.Equal(0, result1.Data.TaggedProduct.DiscountPercentage);

            // Second retrieval: product with discount
            var product2 = new Product
            {
                Id = productId,
                Name = "Test Product",
                Slug = "test-product",
                IsActive = true,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 100000,
                        PriceType = PriceType.REGULAR,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-10)
                    },
                    new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Price = 70000,
                        PriceType = PriceType.SALE,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1)
                    }
                },
                Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = "https://example.com/image1.jpg",
                        ThumbnailUrl = "https://example.com/thumb1.jpg",
                        IsMain = true,
                        DisplayOrder = 1
                    }
                }
            };

            mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId))
                .ReturnsAsync(product2);

            // Get post second time
            var result2 = await marketingPostService.GetPostByIdAsync(postId);

            // Verify discount has been updated to reflect live data
            Assert.True(result2.Data.TaggedProduct.HasDiscount);
            Assert.True(result2.Data.TaggedProduct.DiscountPercentage > 0);
            Assert.Equal(30, result2.Data.TaggedProduct.DiscountPercentage); // (100000 - 70000) / 100000 * 100 = 30%
        }
    }
}
