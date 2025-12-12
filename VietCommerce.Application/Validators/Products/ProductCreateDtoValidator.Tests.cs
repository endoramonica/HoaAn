// Quick validation tests for ProductCreateDtoValidator
// This file demonstrates that the validator works correctly

using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Application.Validators.Products;

/// <summary>
/// Quick validation tests to demonstrate ProductCreateDtoValidator functionality
/// </summary>
public static class ProductCreateDtoValidatorQuickTests
{
    public static async Task RunQuickTests()
    {
        var validator = new ProductCreateDtoValidator();

        // Test 1: Valid product without package
        var dto1 = new ProductCreateDto
        {
            Name = "Regular Product",
            Code = "PROD001",
            Price = 100000,
            StockQuantity = 10,
            IsActive = true
        };
        var result1 = await validator.ValidateAsync(dto1);
        Console.WriteLine($"Test 1 (Valid product without package): {(result1.IsValid ? "PASS" : "FAIL")}");

        // Test 2: Valid package with details
        var dto2 = new ProductCreateDto
        {
            Name = "Mâm Cúng Ông Công Ông Táo",
            Code = "PACKAGE001",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>
            {
                "Cá chép giấy (3 con)",
                "Mũ giấy (3 cái)",
                "Vàng mã (1 bộ)"
            }
        };
        var result2 = await validator.ValidateAsync(dto2);
        Console.WriteLine($"Test 2 (Valid package with details): {(result2.IsValid ? "PASS" : "FAIL")}");

        // Test 3: Valid package with customizable options
        var dto3 = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE002",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };
        var result3 = await validator.ValidateAsync(dto3);
        Console.WriteLine($"Test 3 (Valid package with customizable options): {(result3.IsValid ? "PASS" : "FAIL")}");

        // Test 4: Invalid - empty details list
        var dto4 = new ProductCreateDto
        {
            Name = "Package Product",
            Code = "PACKAGE003",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>()
        };
        var result4 = await validator.ValidateAsync(dto4);
        Console.WriteLine($"Test 4 (Invalid - empty details list): {(!result4.IsValid ? "PASS" : "FAIL")}");

        // Test 5: Invalid - duplicate option IDs
        var dto5 = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE004",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                },
                new CustomizableOptionDto
                {
                    Id = "opt-xoi", // Duplicate
                    Name = "Xôi khác",
                    BaseQuantity = 5,
                    UnitPrice = 50000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };
        var result5 = await validator.ValidateAsync(dto5);
        Console.WriteLine($"Test 5 (Invalid - duplicate option IDs): {(!result5.IsValid ? "PASS" : "FAIL")}");

        // Test 6: Invalid - option with zero unit price
        var dto6 = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE005",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 0, // Invalid
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };
        var result6 = await validator.ValidateAsync(dto6);
        Console.WriteLine($"Test 6 (Invalid - option with zero unit price): {(!result6.IsValid ? "PASS" : "FAIL")}");

        // Test 7: Invalid - min quantity > max quantity
        var dto7 = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE006",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 100, // Invalid: min > max
                    MaxQuantity = 50,
                    Unit = "dĩa"
                }
            }
        };
        var result7 = await validator.ValidateAsync(dto7);
        Console.WriteLine($"Test 7 (Invalid - min quantity > max quantity): {(!result7.IsValid ? "PASS" : "FAIL")}");
    }
}
