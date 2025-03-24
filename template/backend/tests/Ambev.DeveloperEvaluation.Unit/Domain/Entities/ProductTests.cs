using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class ProductTests
    {
        [Fact(DisplayName = "Should mark product as inactive")]
        public void Given_ActiveProduct_When_Deactivate_Then_IsActiveShouldBeFalse()
        {
            // arrange
            var product = ProductTestData.GenerateValidProduct();
            product.IsActive = true;

            // act
            product.Deactivate();

            // assert
            Assert.False(product.IsActive);
            Assert.NotNull(product.UpdatedAt);
            Assert.True(product.UpdatedAt > product.CreatedAt);
        }

        [Fact(DisplayName = "Should pass validation for valid product")]
        public void Given_ValidProduct_When_Validate_Then_ResultShouldBeValid()
        {
            // arrange
            var product = ProductTestData.GenerateValidProduct();

            // act
            var result = product.Validate();

            // assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact(DisplayName = "Should fail validation for invalid product")]
        public void Given_InvalidProduct_When_Validate_Then_ResultShouldBeInvalid()
        {
            // arrange
            var product = new Product
            {
                Name = "",
                Description = null,
                UnitPrice = -5,
                CreatedAt = DateTime.UtcNow
            };

            // act
            var result = product.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact(DisplayName = "Should fail when product name is empty")]
        public void Given_EmptyProductName_When_Validate_Then_ShouldReturnValidationError()
        {
            // arrange
            var product = ProductTestData.GenerateValidProduct();
            product.Name = "";

            // act
            var result = product.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Detail.Contains("Product name is required"));
        }

        [Fact(DisplayName = "Should fail when product name exceeds max length")]
        public void Given_TooLongProductName_When_Validate_Then_ShouldReturnValidationError()
        {
            // arrange
            var product = ProductTestData.GenerateValidProduct();
            product.Name = new string('X', 101);

            // act
            var result = product.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Detail.Contains("cannot exceed 100 characters"));
        }

        [Fact(DisplayName = "Should fail when unit price is negative")]
        public void Given_NegativeUnitPrice_When_Validate_Then_ShouldReturnValidationError()
        {
            // arrange
            var product = ProductTestData.GenerateValidProduct();
            product.UnitPrice = -1;

            // act
            var result = product.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Detail.Contains("Unit price cannot be negative"));
        }
    }
}
