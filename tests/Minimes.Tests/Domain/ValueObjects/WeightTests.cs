using FluentAssertions;
using Minimes.Domain.Enums;
using Minimes.Domain.ValueObjects;
using Xunit;

namespace Minimes.Tests.Domain.ValueObjects;

/// <summary>
/// Weight值对象单元测试
/// </summary>
public class WeightTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidWeight_ShouldCreateWeight()
    {
        // Arrange
        var value = 10.5m;
        var unit = WeightUnit.Kilogram;

        // Act
        var weight = new Weight(value, unit);

        // Assert
        weight.Value.Should().Be(value);
        weight.Unit.Should().Be(unit);
    }

    [Fact]
    public void Constructor_WithZeroWeight_ShouldCreateWeight()
    {
        // Arrange
        var value = 0m;
        var unit = WeightUnit.Kilogram;

        // Act
        var weight = new Weight(value, unit);

        // Assert
        weight.Value.Should().Be(value);
    }

    [Fact]
    public void Constructor_WithNegativeWeight_ShouldThrowArgumentException()
    {
        // Arrange
        var value = -10.5m;
        var unit = WeightUnit.Kilogram;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Weight(value, unit));
    }

    [Theory]
    [InlineData(WeightUnit.Gram)]
    [InlineData(WeightUnit.Kilogram)]
    [InlineData(WeightUnit.Ton)]
    public void Constructor_WithDifferentUnits_ShouldCreateWeight(WeightUnit unit)
    {
        // Arrange
        var value = 10m;

        // Act
        var weight = new Weight(value, unit);

        // Assert
        weight.Unit.Should().Be(unit);
    }

    #endregion

    #region ToKilograms Tests

    [Theory]
    [InlineData(1000, WeightUnit.Gram, 1)]
    [InlineData(1, WeightUnit.Kilogram, 1)]
    [InlineData(1, WeightUnit.Ton, 1000)]
    [InlineData(500, WeightUnit.Gram, 0.5)]
    [InlineData(2.5, WeightUnit.Kilogram, 2.5)]
    [InlineData(0.5, WeightUnit.Ton, 500)]
    public void ToKilograms_ShouldConvertCorrectly(decimal value, WeightUnit unit, decimal expectedKg)
    {
        // Arrange
        var weight = new Weight(value, unit);

        // Act
        var result = weight.ToKilograms();

        // Assert
        result.Should().Be(expectedKg);
    }

    [Fact]
    public void ToKilograms_WithZeroWeight_ShouldReturnZero()
    {
        // Arrange
        var weight = new Weight(0, WeightUnit.Kilogram);

        // Act
        var result = weight.ToKilograms();

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Equals Tests

    [Fact]
    public void Equals_WithSameWeightDifferentUnit_ShouldReturnTrue()
    {
        // Arrange
        var weight1 = new Weight(1, WeightUnit.Kilogram);
        var weight2 = new Weight(1000, WeightUnit.Gram);

        // Act & Assert
        weight1.Equals(weight2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentWeight_ShouldReturnFalse()
    {
        // Arrange
        var weight1 = new Weight(1, WeightUnit.Kilogram);
        var weight2 = new Weight(2, WeightUnit.Kilogram);

        // Act & Assert
        weight1.Equals(weight2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameWeightSameUnit_ShouldReturnTrue()
    {
        // Arrange
        var weight1 = new Weight(10.5m, WeightUnit.Kilogram);
        var weight2 = new Weight(10.5m, WeightUnit.Kilogram);

        // Act & Assert
        weight1.Equals(weight2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var weight = new Weight(10, WeightUnit.Kilogram);
        var someValue = 10;

        // Act & Assert
        weight.Equals(someValue).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var weight = new Weight(10, WeightUnit.Kilogram);

        // Act & Assert
        weight.Equals(null).Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameWeight_ShouldReturnSameHash()
    {
        // Arrange
        var weight1 = new Weight(10.5m, WeightUnit.Kilogram);
        var weight2 = new Weight(10.5m, WeightUnit.Kilogram);

        // Act & Assert
        weight1.GetHashCode().Should().Be(weight2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentWeight_ShouldReturnDifferentHash()
    {
        // Arrange
        var weight1 = new Weight(10, WeightUnit.Kilogram);
        var weight2 = new Weight(20, WeightUnit.Kilogram);

        // Act & Assert
        weight1.GetHashCode().Should().NotBe(weight2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeUsableInDictionary()
    {
        // Arrange
        var weight1 = new Weight(10, WeightUnit.Kilogram);
        var weight2 = new Weight(10, WeightUnit.Kilogram); // 值相同，Equals返回true
        var weight3 = new Weight(20, WeightUnit.Kilogram);

        var dictionary = new Dictionary<Weight, string>
        {
            { weight1, "10kg物品" },
            { weight3, "20kg物品" }
        };

        // Act & Assert - weight2因为值相同，能在字典中找到
        dictionary.ContainsKey(weight1).Should().BeTrue();
        dictionary.ContainsKey(weight2).Should().BeTrue(); // 值对象相等性
        dictionary[weight2].Should().Be("10kg物品");

        dictionary.ContainsKey(weight3).Should().BeTrue();
        dictionary[weight3].Should().Be("20kg物品");
    }

    #endregion

    #region ToString Tests

    [Theory]
    [InlineData(1000, WeightUnit.Gram, "1000.00 g")]
    [InlineData(10.5, WeightUnit.Kilogram, "10.50 kg")]
    [InlineData(1.5, WeightUnit.Ton, "1.50 t")]
    public void ToString_ShouldReturnFormattedString(decimal value, WeightUnit unit, string expected)
    {
        // Arrange
        var weight = new Weight(value, unit);

        // Act
        var result = weight.ToString();

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Weight_WithVerySmallValue_ShouldWork()
    {
        // Arrange
        var value = 0.001m;
        var unit = WeightUnit.Kilogram;

        // Act
        var weight = new Weight(value, unit);

        // Assert
        weight.Value.Should().Be(value);
        weight.ToKilograms().Should().Be(value);
    }

    [Fact]
    public void Weight_WithVeryLargeValue_ShouldWork()
    {
        // Arrange
        var value = 999999.99m;
        var unit = WeightUnit.Kilogram;

        // Act
        var weight = new Weight(value, unit);

        // Assert
        weight.Value.Should().Be(value);
    }

    [Fact]
    public void Weight_ConversionChain_ShouldBeConsistent()
    {
        // 1 ton = 1000 kg = 1000000 gram
        // Arrange
        var gramWeight = new Weight(1000000, WeightUnit.Gram);
        var kilogramWeight = new Weight(1000, WeightUnit.Kilogram);
        var tonWeight = new Weight(1, WeightUnit.Ton);

        // Act & Assert
        gramWeight.ToKilograms().Should().Be(kilogramWeight.ToKilograms());
        kilogramWeight.ToKilograms().Should().Be(tonWeight.ToKilograms());
        gramWeight.Equals(kilogramWeight).Should().BeTrue();
        kilogramWeight.Equals(tonWeight).Should().BeTrue();
    }

    #endregion
}
