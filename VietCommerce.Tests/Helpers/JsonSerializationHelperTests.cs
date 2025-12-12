using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Helpers;
using Xunit;

namespace VietCommerce.Tests.Helpers;

/// <summary>
/// Unit tests for JsonSerializationHelper
/// Tests JSON serialization and deserialization for package products
/// Validates: Requirements 1.4, 7.4
/// </summary>
public class JsonSerializationHelperTests
{
    #region Details Serialization Tests

    [Fact]
    public void SerializeDetails_WithValidDetails_ShouldReturnJson()
    {
        // Arrange
        var details = new List<string>
        {
            "Cá chép giấy (3 con)",
            "Mũ giấy (3 cái)",
            "Vàng mã (1 bộ)",
            "Hương đèn (1 bộ)",
            "Hoa quả (1 mâm)",
            "Bánh kẹo (1 hộp)"
        };

        // Act
        var json = JsonSerializationHelper.SerializeDetails(details);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("Cá chép giấy", json);
        Assert.Contains("Mũ giấy", json);
        Assert.Contains("Vàng mã", json);
    }

    [Fact]
    public void SerializeDetails_WithNullDetails_ShouldReturnNull()
    {
        // Act
        var json = JsonSerializationHelper.SerializeDetails(null);

        // Assert
        Assert.Null(json);
    }

    [Fact]
    public void SerializeDetails_WithEmptyDetails_ShouldReturnNull()
    {
        // Arrange
        var details = new List<string>();

        // Act
        var json = JsonSerializationHelper.SerializeDetails(details);

        // Assert
        Assert.Null(json);
    }

    [Fact]
    public void DeserializeDetails_WithValidJson_ShouldReturnDetails()
    {
        // Arrange
        var originalDetails = new List<string>
        {
            "Cá chép giấy (3 con)",
            "Mũ giấy (3 cái)",
            "Vàng mã (1 bộ)"
        };
        var json = JsonSerializationHelper.SerializeDetails(originalDetails);

        // Act
        var deserialized = JsonSerializationHelper.DeserializeDetails(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(3, deserialized.Count);
        Assert.Equal("Cá chép giấy (3 con)", deserialized[0]);
        Assert.Equal("Mũ giấy (3 cái)", deserialized[1]);
        Assert.Equal("Vàng mã (1 bộ)", deserialized[2]);
    }

    [Fact]
    public void DeserializeDetails_WithNullJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeDetails(null);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    [Fact]
    public void DeserializeDetails_WithEmptyJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeDetails(string.Empty);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    [Fact]
    public void DeserializeDetails_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeDetails("invalid json");

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    #endregion

    #region Customizable Options Serialization Tests

    [Fact]
    public void SerializeCustomizableOptions_WithValidOptions_ShouldReturnJson()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            },
            new()
            {
                Id = "opt-che",
                Name = "Chè trôi nước",
                BaseQuantity = 5,
                UnitPrice = 35000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "chén"
            }
        };

        // Act
        var json = JsonSerializationHelper.SerializeCustomizableOptions(options);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("opt-xoi", json);
        Assert.Contains("Xôi gấc đậu xanh", json);
        Assert.Contains("opt-che", json);
        Assert.Contains("Chè trôi nước", json);
    }

    [Fact]
    public void SerializeCustomizableOptions_WithNullOptions_ShouldReturnNull()
    {
        // Act
        var json = JsonSerializationHelper.SerializeCustomizableOptions(null);

        // Assert
        Assert.Null(json);
    }

    [Fact]
    public void SerializeCustomizableOptions_WithEmptyOptions_ShouldReturnNull()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>();

        // Act
        var json = JsonSerializationHelper.SerializeCustomizableOptions(options);

        // Assert
        Assert.Null(json);
    }

    [Fact]
    public void DeserializeCustomizableOptions_WithValidJson_ShouldReturnOptions()
    {
        // Arrange
        var originalOptions = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };
        var json = JsonSerializationHelper.SerializeCustomizableOptions(originalOptions);

        // Act
        var deserialized = JsonSerializationHelper.DeserializeCustomizableOptions(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Single(deserialized);
        Assert.Equal("opt-xoi", deserialized[0].Id);
        Assert.Equal("Xôi gấc đậu xanh", deserialized[0].Name);
        Assert.Equal(5, deserialized[0].BaseQuantity);
        Assert.Equal(45000, deserialized[0].UnitPrice);
        Assert.Equal(5, deserialized[0].MinQuantity);
        Assert.Equal(100, deserialized[0].MaxQuantity);
        Assert.Equal("dĩa", deserialized[0].Unit);
    }

    [Fact]
    public void DeserializeCustomizableOptions_WithNullJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeCustomizableOptions(null);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    [Fact]
    public void DeserializeCustomizableOptions_WithEmptyJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeCustomizableOptions(string.Empty);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    [Fact]
    public void DeserializeCustomizableOptions_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Act
        var deserialized = JsonSerializationHelper.DeserializeCustomizableOptions("invalid json");

        // Assert
        Assert.NotNull(deserialized);
        Assert.Empty(deserialized);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void RoundTrip_Details_ShouldPreserveData()
    {
        // Arrange
        var originalDetails = new List<string>
        {
            "Item 1",
            "Item 2",
            "Item 3"
        };

        // Act
        var json = JsonSerializationHelper.SerializeDetails(originalDetails);
        var deserialized = JsonSerializationHelper.DeserializeDetails(json);

        // Assert
        Assert.Equal(originalDetails.Count, deserialized.Count);
        for (int i = 0; i < originalDetails.Count; i++)
        {
            Assert.Equal(originalDetails[i], deserialized[i]);
        }
    }

    [Fact]
    public void RoundTrip_CustomizableOptions_ShouldPreserveData()
    {
        // Arrange
        var originalOptions = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-1",
                Name = "Option 1",
                BaseQuantity = 5,
                UnitPrice = 50000,
                MinQuantity = 1,
                MaxQuantity = 100,
                Unit = "cái"
            },
            new()
            {
                Id = "opt-2",
                Name = "Option 2",
                BaseQuantity = 10,
                UnitPrice = 75000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "bộ"
            }
        };

        // Act
        var json = JsonSerializationHelper.SerializeCustomizableOptions(originalOptions);
        var deserialized = JsonSerializationHelper.DeserializeCustomizableOptions(json);

        // Assert
        Assert.Equal(originalOptions.Count, deserialized.Count);
        for (int i = 0; i < originalOptions.Count; i++)
        {
            Assert.Equal(originalOptions[i].Id, deserialized[i].Id);
            Assert.Equal(originalOptions[i].Name, deserialized[i].Name);
            Assert.Equal(originalOptions[i].BaseQuantity, deserialized[i].BaseQuantity);
            Assert.Equal(originalOptions[i].UnitPrice, deserialized[i].UnitPrice);
            Assert.Equal(originalOptions[i].MinQuantity, deserialized[i].MinQuantity);
            Assert.Equal(originalOptions[i].MaxQuantity, deserialized[i].MaxQuantity);
            Assert.Equal(originalOptions[i].Unit, deserialized[i].Unit);
        }
    }

    #endregion
}
