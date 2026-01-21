using FluentAssertions;
using Minimes.Domain.ValueObjects;
using Xunit;

namespace Minimes.Tests.Domain.ValueObjects;

/// <summary>
/// Barcode值对象单元测试
/// </summary>
public class BarcodeTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidBarcode_ShouldCreateBarcode()
    {
        // Arrange
        var barcodeValue = "6901012341234";

        // Act
        var barcode = new Barcode(barcodeValue);

        // Assert
        barcode.Value.Should().Be(barcodeValue);
    }

    [Fact]
    public void Constructor_WithWhitespace_ShouldTrimValue()
    {
        // Arrange
        var barcodeValue = "  6901012341234  ";
        var expectedValue = "6901012341234";

        // Act
        var barcode = new Barcode(barcodeValue);

        // Assert
        barcode.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyOrNullBarcode_ShouldThrowArgumentException(string? barcodeValue)
    {
        // Act & Assert
        // 这里用! 操作符强制传递null值给构造函数
        Assert.Throws<ArgumentException>(() => new Barcode(barcodeValue!));
    }

    #endregion

    #region Equals Tests

    [Fact]
    public void Equals_WithSameBarcodeValue_ShouldReturnTrue()
    {
        // Arrange
        var barcode1 = new Barcode("6901012341234");
        var barcode2 = new Barcode("6901012341234");

        // Act & Assert
        barcode1.Equals(barcode2).Should().BeTrue();
        (barcode1 == barcode2).Should().BeFalse(); // 没有重载==操作符，只能通过Equals
    }

    [Fact]
    public void Equals_WithDifferentBarcodeValue_ShouldReturnFalse()
    {
        // Arrange
        var barcode1 = new Barcode("6901012341234");
        var barcode2 = new Barcode("6901012341235");

        // Act & Assert
        barcode1.Equals(barcode2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var barcode = new Barcode("6901012341234");
        var someString = "6901012341234";

        // Act & Assert
        barcode.Equals(someString).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var barcode = new Barcode("6901012341234");

        // Act & Assert
        barcode.Equals(null).Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameBarcodeValue_ShouldReturnSameHash()
    {
        // Arrange
        var barcode1 = new Barcode("6901012341234");
        var barcode2 = new Barcode("6901012341234");

        // Act & Assert
        barcode1.GetHashCode().Should().Be(barcode2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentBarcodeValue_ShouldReturnDifferentHash()
    {
        // Arrange
        var barcode1 = new Barcode("6901012341234");
        var barcode2 = new Barcode("6901012341235");

        // Act & Assert
        barcode1.GetHashCode().Should().NotBe(barcode2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeUsableInDictionary()
    {
        // Arrange
        var barcode1 = new Barcode("6901012341234");
        var barcode2 = new Barcode("6901012341234"); // 值相同，Equals返回true
        var barcode3 = new Barcode("6901012341235");

        var dictionary = new Dictionary<Barcode, string>
        {
            { barcode1, "商品1" },
            { barcode3, "商品2" }
        };

        // Act & Assert - barcode2因为值相同，能在字典中找到
        dictionary.ContainsKey(barcode2).Should().BeTrue(); // 值对象相等性
        dictionary.TryGetValue(barcode2, out var value).Should().BeTrue();
        value.Should().Be("商品1");

        dictionary.ContainsKey(barcode3).Should().BeTrue();
        dictionary[barcode3].Should().Be("商品2");
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnBarcodeValue()
    {
        // Arrange
        var barcodeValue = "6901012341234";
        var barcode = new Barcode(barcodeValue);

        // Act
        var result = barcode.ToString();

        // Assert
        result.Should().Be(barcodeValue);
    }

    #endregion

}
