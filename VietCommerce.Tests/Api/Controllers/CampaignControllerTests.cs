using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Api.Controllers;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using System.Security.Claims;

namespace VietCommerce.Tests.Api.Controllers
{
    /// <summary>
    /// Unit tests for CampaignController API endpoints
    /// Validates: Requirements 1.1, 1.2, 1.3, 1.4, 1.5, 8.1, 8.2, 8.3, 8.4
    /// </summary>
    public class CampaignControllerTests
    {
        private readonly Mock<ICampaignService> _mockCampaignService;
        private readonly Mock<ILogger<CampaignController>> _mockLogger;
        private readonly CampaignController _controller;

        public CampaignControllerTests()
        {
            _mockCampaignService = new Mock<ICampaignService>();
            _mockLogger = new Mock<ILogger<CampaignController>>();
            _controller = new CampaignController(_mockCampaignService.Object, _mockLogger.Object);

            // Setup user context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("StoreId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region CreateCampaign Tests

        [Fact]
        public async Task CreateCampaign_WithValidData_ReturnsOkWithCampaign()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "Summer Sale",
                Description = "Summer promotional campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 10000000
            };

            var campaignDto = new CampaignDto
            {
                Id = Guid.NewGuid(),
                StoreId = createDto.StoreId,
                CampaignName = createDto.CampaignName,
                Status = CampaignStatus.DRAFT,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Budget = createDto.Budget
            };

            var response = ApiResponse<CampaignDto>.SuccessResponse(campaignDto, "Campaign created successfully");
            _mockCampaignService.Setup(s => s.CreateCampaignAsync(It.IsAny<CreateCampaignDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCampaign(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.NotNull(returnedResponse.Data);
            Assert.Equal(createDto.CampaignName, returnedResponse.Data.CampaignName);
        }

        [Fact]
        public async Task CreateCampaign_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "Invalid Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(1), // EndDate < StartDate
                Budget = 10000000
            };

            // Act
            var result = await _controller.CreateCampaign(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("EndDate must be greater than StartDate", returnedResponse.Message);
        }

        [Fact]
        public async Task CreateCampaign_WithNegativeBudget_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "Invalid Budget Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = -1000 // Negative budget
            };

            // Act
            var result = await _controller.CreateCampaign(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Budget must be non-negative", returnedResponse.Message);
        }

        [Fact]
        public async Task CreateCampaign_WithEmptyCampaignName_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "", // Empty name
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 10000000
            };

            // Act
            var result = await _controller.CreateCampaign(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Campaign name is required", returnedResponse.Message);
        }

        #endregion

        #region GetCampaigns Tests

        [Fact]
        public async Task GetCampaigns_WithValidParameters_ReturnsOkWithPaginatedList()
        {
            // Arrange
            var campaigns = new List<CampaignDto>
            {
                new CampaignDto { Id = Guid.NewGuid(), CampaignName = "Campaign 1", Status = CampaignStatus.DRAFT },
                new CampaignDto { Id = Guid.NewGuid(), CampaignName = "Campaign 2", Status = CampaignStatus.ACTIVE }
            };

            var paginatedResult = new PaginatedResult<CampaignDto>
            {
                Items = campaigns,
                PageNumber = 1,
                PageSize = 10,
                TotalItems = 2,
                TotalPages = 1
            };

            var response = ApiResponse<PaginatedResult<CampaignDto>>.SuccessResponse(paginatedResult, "Campaigns retrieved successfully");
            _mockCampaignService.Setup(s => s.GetCampaignsAsync(It.IsAny<GetCampaignsQueryDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetCampaigns(pageNumber: 1, pageSize: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<PaginatedResult<CampaignDto>>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.NotNull(returnedResponse.Data);
            Assert.Equal(2, returnedResponse.Data.Items.Count());
        }

        [Fact]
        public async Task GetCampaigns_WithInvalidPageNumber_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCampaigns(pageNumber: 0, pageSize: 10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<PaginatedResult<CampaignDto>>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Page number must be greater than 0", returnedResponse.Message);
        }

        [Fact]
        public async Task GetCampaigns_WithInvalidPageSize_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCampaigns(pageNumber: 1, pageSize: 101);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<PaginatedResult<CampaignDto>>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Page size must be between 1 and 100", returnedResponse.Message);
        }

        #endregion

        #region GetCampaignById Tests

        [Fact]
        public async Task GetCampaignById_WithValidId_ReturnsOkWithCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaignDto = new CampaignDto
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT
            };

            var response = ApiResponse<CampaignDto>.SuccessResponse(campaignDto, "Campaign retrieved successfully");
            _mockCampaignService.Setup(s => s.GetCampaignByIdAsync(campaignId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.NotNull(returnedResponse.Data);
            Assert.Equal(campaignId, returnedResponse.Data.Id);
        }

        [Fact]
        public async Task GetCampaignById_WithInvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCampaignById(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Campaign ID cannot be empty", returnedResponse.Message);
        }

        [Fact]
        public async Task GetCampaignById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var response = ApiResponse<CampaignDto>.FailureResponse("Campaign not found");
            _mockCampaignService.Setup(s => s.GetCampaignByIdAsync(campaignId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(notFoundResult.Value);
            Assert.False(returnedResponse.Success);
        }

        #endregion

        #region UpdateCampaign Tests

        [Fact]
        public async Task UpdateCampaign_WithValidData_ReturnsOkWithUpdatedCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var updateDto = new UpdateCampaignDto
            {
                CampaignName = "Updated Campaign Name",
                Budget = 20000000
            };

            var updatedCampaignDto = new CampaignDto
            {
                Id = campaignId,
                CampaignName = updateDto.CampaignName,
                Budget = updateDto.Budget.Value,
                Status = CampaignStatus.DRAFT
            };

            var response = ApiResponse<CampaignDto>.SuccessResponse(updatedCampaignDto, "Campaign updated successfully");
            _mockCampaignService.Setup(s => s.UpdateCampaignAsync(campaignId, It.IsAny<UpdateCampaignDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCampaign(campaignId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.NotNull(returnedResponse.Data);
            Assert.Equal(updateDto.CampaignName, returnedResponse.Data.CampaignName);
        }

        [Fact]
        public async Task UpdateCampaign_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var updateDto = new UpdateCampaignDto
            {
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(1) // EndDate < StartDate
            };

            // Act
            var result = await _controller.UpdateCampaign(campaignId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("EndDate must be greater than StartDate", returnedResponse.Message);
        }

        [Fact]
        public async Task UpdateCampaign_WithNegativeBudget_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var updateDto = new UpdateCampaignDto
            {
                Budget = -1000 // Negative budget
            };

            // Act
            var result = await _controller.UpdateCampaign(campaignId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Budget must be non-negative", returnedResponse.Message);
        }

        [Fact]
        public async Task UpdateCampaign_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var updateDto = new UpdateCampaignDto { CampaignName = "Updated" };
            var response = ApiResponse<CampaignDto>.FailureResponse("Campaign not found");
            _mockCampaignService.Setup(s => s.UpdateCampaignAsync(campaignId, It.IsAny<UpdateCampaignDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCampaign(campaignId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        #endregion

        #region DeleteCampaign Tests

        [Fact]
        public async Task DeleteCampaign_WithValidId_ReturnsOkWithTrue()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var response = ApiResponse<bool>.SuccessResponse(true, "Campaign deleted successfully");
            _mockCampaignService.Setup(s => s.DeleteCampaignAsync(campaignId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCampaign(campaignId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.True(returnedResponse.Data);
        }

        [Fact]
        public async Task DeleteCampaign_WithInvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteCampaign(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Campaign ID cannot be empty", returnedResponse.Message);
        }

        [Fact]
        public async Task DeleteCampaign_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var response = ApiResponse<bool>.FailureResponse("Campaign not found");
            _mockCampaignService.Setup(s => s.DeleteCampaignAsync(campaignId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCampaign(campaignId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        #endregion

        #region ChangeCampaignStatus Tests

        [Fact]
        public async Task ChangeCampaignStatus_WithValidStatus_ReturnsOkWithUpdatedCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var statusDto = new ChangeCampaignStatusDto { NewStatus = (int)CampaignStatus.ACTIVE };
            var updatedCampaignDto = new CampaignDto
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.ACTIVE
            };

            var response = ApiResponse<CampaignDto>.SuccessResponse(updatedCampaignDto, "Campaign status changed successfully");
            _mockCampaignService.Setup(s => s.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ChangeCampaignStatus(campaignId, statusDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal(CampaignStatus.ACTIVE, returnedResponse.Data.Status);
        }

        [Fact]
        public async Task ChangeCampaignStatus_WithInvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var statusDto = new ChangeCampaignStatusDto { NewStatus = 999 }; // Invalid status

            // Act
            var result = await _controller.ChangeCampaignStatus(campaignId, statusDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(badRequestResult.Value);
            Assert.False(returnedResponse.Success);
            Assert.Contains("Invalid campaign status", returnedResponse.Message);
        }

        [Fact]
        public async Task ChangeCampaignStatus_WithBusinessRuleViolation_ReturnsConflict()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var statusDto = new ChangeCampaignStatusDto { NewStatus = (int)CampaignStatus.ACTIVE };
            var response = ApiResponse<CampaignDto>.FailureResponse("Cannot activate campaign. Ensure CampaignName is set, Budget >= 0, and EndDate > StartDate");
            _mockCampaignService.Setup(s => s.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ChangeCampaignStatus(campaignId, statusDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            var returnedResponse = Assert.IsType<ApiResponse<CampaignDto>>(conflictResult.Value);
            Assert.False(returnedResponse.Success);
        }

        #endregion
    }
}
