# Backend Implementation - Booking Feature

## 📋 Tổng Quan

Hướng dẫn implement Booking API trên backend .NET.

## 🏗️ Cấu Trúc Thư Mục

```
VietCommerce.API/
├── Controllers/
│   └── BookingController.cs
├── Models/
│   ├── Requests/
│   │   └── BookingCreateRequest.cs
│   ├── Responses/
│   │   └── BookingResponse.cs
│   └── Entities/
│       ├── LunarDateInfo.cs
│       └── Booking.cs
├── Services/
│   ├── Interfaces/
│   │   └── IBookingService.cs
│   └── Implementations/
│       └── BookingService.cs
├── Repositories/
│   ├── Interfaces/
│   │   └── IBookingRepository.cs
│   └── Implementations/
│       └── BookingRepository.cs
└── Data/
    └── ApplicationDbContext.cs (update)
```

## 📝 DTOs

### LunarDateInfo.cs

```csharp
namespace VietCommerce.API.Models
{
    public class LunarDateInfo
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string HeavenlyStems { get; set; }
        public string EarthlyBranches { get; set; }
        public string SexagenaryCycle { get; set; }
    }
}
```

### BookingCreateRequest.cs

```csharp
namespace VietCommerce.API.Models.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class BookingCreateRequest
    {
        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "ServiceId is required")]
        public string ServiceId { get; set; }

        [Required(ErrorMessage = "SolarDate is required")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "SolarDate must be in YYYY-MM-DD format")]
        public string SolarDate { get; set; }

        [Required(ErrorMessage = "LunarDate is required")]
        public LunarDateInfo LunarDate { get; set; }

        [Required(ErrorMessage = "CustomerName is required")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "CustomerName must be between 2 and 255 characters")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "CustomerPhone is required")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "CustomerPhone must be 10-11 digits")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "CustomerEmail is required")]
        [EmailAddress(ErrorMessage = "CustomerEmail must be a valid email address")]
        public string CustomerEmail { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
```

### BookingResponse.cs

```csharp
namespace VietCommerce.API.Models.Responses
{
    using System;

    public class BookingResponse
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string ServiceId { get; set; }
        public string SolarDate { get; set; }
        public LunarDateInfo LunarDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

## 🗄️ Entity

### Booking.cs

```csharp
namespace VietCommerce.API.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Bookings")]
    public class Booking
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string ServiceId { get; set; }

        [Required]
        public DateTime SolarDate { get; set; }

        // Lunar date fields
        [Required]
        public int LunarDay { get; set; }

        [Required]
        public int LunarMonth { get; set; }

        [Required]
        public int LunarYear { get; set; }

        public string HeavenlyStems { get; set; }
        public string EarthlyBranches { get; set; }
        public string SexagenaryCycle { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomerEmail { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

## 🔧 Repository

### IBookingRepository.cs

```csharp
namespace VietCommerce.API.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VietCommerce.API.Models.Entities;

    public interface IBookingRepository
    {
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> GetByIdAsync(string id);
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(string customerId);
        Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Booking> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(string id);
    }
}
```

### BookingRepository.cs

```csharp
namespace VietCommerce.API.Repositories.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using VietCommerce.API.Data;
    using VietCommerce.API.Models.Entities;
    using VietCommerce.API.Repositories.Interfaces;

    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> GetByIdAsync(string id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.SolarDate >= startDate && b.SolarDate <= endDate)
                .OrderBy(b => b.SolarDate)
                .ToListAsync();
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            booking.UpdatedAt = DateTime.UtcNow;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var booking = await GetByIdAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

## 🛠️ Service

### IBookingService.cs

```csharp
namespace VietCommerce.API.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VietCommerce.API.Models.Requests;
    using VietCommerce.API.Models.Responses;

    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request);
        Task<BookingResponse> GetBookingAsync(string bookingId);
        Task<IEnumerable<BookingResponse>> GetCustomerBookingsAsync(string customerId);
        Task<IEnumerable<BookingResponse>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<BookingResponse> CancelBookingAsync(string bookingId);
    }
}
```

### BookingService.cs

```csharp
namespace VietCommerce.API.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VietCommerce.API.Models.Entities;
    using VietCommerce.API.Models.Requests;
    using VietCommerce.API.Models.Responses;
    using VietCommerce.API.Repositories.Interfaces;
    using VietCommerce.API.Services.Interfaces;

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private const string FIXED_CUSTOMER_ID = "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7";

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request)
        {
            // Validate customer ID
            if (request.CustomerId != FIXED_CUSTOMER_ID)
            {
                throw new InvalidOperationException("Invalid customer ID");
            }

            // Parse solar date
            if (!DateTime.TryParse(request.SolarDate, out var solarDate))
            {
                throw new ArgumentException("Invalid solar date format");
            }

            // Create booking entity
            var booking = new Booking
            {
                CustomerId = FIXED_CUSTOMER_ID,
                ServiceId = request.ServiceId,
                SolarDate = solarDate,
                LunarDay = request.LunarDate.Day,
                LunarMonth = request.LunarDate.Month,
                LunarYear = request.LunarDate.Year,
                HeavenlyStems = request.LunarDate.HeavenlyStems,
                EarthlyBranches = request.LunarDate.EarthlyBranches,
                SexagenaryCycle = request.LunarDate.SexagenaryCycle,
                CustomerName = request.CustomerName.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                CustomerEmail = request.CustomerEmail.Trim(),
                Notes = request.Notes?.Trim(),
                Status = "pending",
            };

            var createdBooking = await _repository.CreateAsync(booking);
            return MapToResponse(createdBooking);
        }

        public async Task<BookingResponse> GetBookingAsync(string bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
            }

            return MapToResponse(booking);
        }

        public async Task<IEnumerable<BookingResponse>> GetCustomerBookingsAsync(string customerId)
        {
            var bookings = await _repository.GetByCustomerIdAsync(customerId);
            return bookings.Select(MapToResponse);
        }

        public async Task<IEnumerable<BookingResponse>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _repository.GetByDateRangeAsync(startDate, endDate);
            return bookings.Select(MapToResponse);
        }

        public async Task<BookingResponse> CancelBookingAsync(string bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
            }

            booking.Status = "cancelled";
            var updatedBooking = await _repository.UpdateAsync(booking);
            return MapToResponse(updatedBooking);
        }

        private BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                ServiceId = booking.ServiceId,
                SolarDate = booking.SolarDate.ToString("yyyy-MM-dd"),
                LunarDate = new LunarDateInfo
                {
                    Day = booking.LunarDay,
                    Month = booking.LunarMonth,
                    Year = booking.LunarYear,
                    HeavenlyStems = booking.HeavenlyStems,
                    EarthlyBranches = booking.EarthlyBranches,
                    SexagenaryCycle = booking.SexagenaryCycle,
                },
                CustomerName = booking.CustomerName,
                CustomerPhone = booking.CustomerPhone,
                CustomerEmail = booking.CustomerEmail,
                Notes = booking.Notes,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
            };
        }
    }
}
```

## 🎮 Controller

### BookingController.cs

```csharp
namespace VietCommerce.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VietCommerce.API.Models.Requests;
    using VietCommerce.API.Models.Responses;
    using VietCommerce.API.Services.Interfaces;

    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(typeof(BookingResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] BookingCreateRequest request)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(request);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{bookingId}")]
        [ProducesResponseType(typeof(BookingResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<BookingResponse>> GetBooking(string bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingAsync(bookingId);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all bookings for a customer
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetCustomerBookings(string customerId)
        {
            var bookings = await _bookingService.GetCustomerBookingsAsync(customerId);
            return Ok(bookings);
        }

        /// <summary>
        /// Get bookings by date range
        /// </summary>
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookingsByDateRange(
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            {
                return BadRequest(new { message = "Invalid date format" });
            }

            var bookings = await _bookingService.GetBookingsByDateRangeAsync(start, end);
            return Ok(bookings);
        }

        /// <summary>
        /// Cancel a booking
        /// </summary>
        [HttpPost("{bookingId}/cancel")]
        [ProducesResponseType(typeof(BookingResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<BookingResponse>> CancelBooking(string bookingId)
        {
            try
            {
                var booking = await _bookingService.CancelBookingAsync(bookingId);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
```

## 📦 Dependency Injection

### Startup.cs / Program.cs

```csharp
// Add to ConfigureServices or builder.Services
services.AddScoped<IBookingRepository, BookingRepository>();
services.AddScoped<IBookingService, BookingService>();
```

## 🗄️ Database Migration

### Create Migration

```bash
dotnet ef migrations add AddBookingTable
dotnet ef database update
```

### Migration File

```csharp
namespace VietCommerce.API.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddBookingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolarDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LunarDay = table.Column<int>(type: "int", nullable: false),
                    LunarMonth = table.Column<int>(type: "int", nullable: false),
                    LunarYear = table.Column<int>(type: "int", nullable: false),
                    HeavenlyStems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EarthlyBranches = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SexagenaryCycle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SolarDate",
                table: "Bookings",
                column: "SolarDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Bookings");
        }
    }
}
```

## 🧪 Unit Tests

```csharp
namespace VietCommerce.API.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;
    using VietCommerce.API.Models.Entities;
    using VietCommerce.API.Models.Requests;
    using VietCommerce.API.Models;
    using VietCommerce.API.Repositories.Interfaces;
    using VietCommerce.API.Services.Implementations;

    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepository;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_WithValidRequest_ReturnsBookingResponse()
        {
            // Arrange
            var request = new BookingCreateRequest
            {
                CustomerId = "D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7",
                ServiceId = "svc-001",
                SolarDate = "2025-01-29",
                LunarDate = new LunarDateInfo
                {
                    Day = 1,
                    Month = 1,
                    Year = 2025,
                    HeavenlyStems = "Ất",
                    EarthlyBranches = "Tỵ",
                    SexagenaryCycle = "Ất Tỵ",
                },
                CustomerName = "Test User",
                CustomerPhone = "0912345678",
                CustomerEmail = "test@example.com",
            };

            var booking = new Booking
            {
                Id = "booking-001",
                CustomerId = request.CustomerId,
                ServiceId = request.ServiceId,
                SolarDate = DateTime.Parse(request.SolarDate),
                LunarDay = request.LunarDate.Day,
                LunarMonth = request.LunarDate.Month,
                LunarYear = request.LunarDate.Year,
                HeavenlyStems = request.LunarDate.HeavenlyStems,
                EarthlyBranches = request.LunarDate.EarthlyBranches,
                SexagenaryCycle = request.LunarDate.SexagenaryCycle,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Status = "pending",
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Booking>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CreateBookingAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("booking-001", result.Id);
            Assert.Equal("pending", result.Status);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Once);
        }
    }
}
```

## 📋 Checklist

- [ ] Tạo DTOs (LunarDateInfo, BookingCreateRequest, BookingResponse)
- [ ] Tạo Entity (Booking)
- [ ] Tạo Repository (IBookingRepository, BookingRepository)
- [ ] Tạo Service (IBookingService, BookingService)
- [ ] Tạo Controller (BookingController)
- [ ] Thêm Dependency Injection
- [ ] Tạo Database Migration
- [ ] Update DbContext
- [ ] Viết Unit Tests
- [ ] Update Swagger/OpenAPI
- [ ] Test API endpoints
- [ ] Deploy
