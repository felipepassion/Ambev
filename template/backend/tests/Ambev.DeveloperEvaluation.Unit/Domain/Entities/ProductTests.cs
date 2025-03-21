using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class ProductTests
{
    [Fact(DisplayName = "Should mark product as inactive")]
    public void Given_ActiveProduct_When_Deactivate_Then_IsActiveShouldBeFalse()
    {
        var product = ProductTestData.GenerateValidProduct();
        product.IsActive = true;

        product.Deactivate();

        Assert.False(product.IsActive);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(product.UpdatedAt > product.CreatedAt);
    }

    [Fact(DisplayName = "Should pass validation for valid product")]
    public void Given_ValidProduct_When_Validate_Then_ResultShouldBeValid()
    {
        var product = ProductTestData.GenerateValidProduct();

        var result = product.Validate();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Should fail validation for invalid product")]
    public void Given_InvalidProduct_When_Validate_Then_ResultShouldBeInvalid()
    {
        var product = new Product
        {
            Name = "",
            Description = null,
            UnitPrice = -5,
            CreatedAt = DateTime.UtcNow
        };

        var result = product.Validate();

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact(DisplayName = "Should fail when product name is empty")]
    public void Given_EmptyProductName_When_Validate_Then_ShouldReturnValidationError()
    {
        var product = ProductTestData.GenerateValidProduct();
        product.Name = "";

        var result = product.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("Product name is required"));
    }

    [Fact(DisplayName = "Should fail when product name exceeds max length")]
    public void Given_TooLongProductName_When_Validate_Then_ShouldReturnValidationError()
    {
        var product = ProductTestData.GenerateValidProduct();
        product.Name = new string('X', 101);

        var result = product.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("cannot exceed 100 characters"));
    }


    [Fact(DisplayName = "Should fail when unit price is negative")]
    public void Given_NegativeUnitPrice_When_Validate_Then_ShouldReturnValidationError()
    {
        var product = ProductTestData.GenerateValidProduct();
        product.UnitPrice = -1;

        var result = product.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail.Contains("Unit price cannot be negative"));
    }

}
