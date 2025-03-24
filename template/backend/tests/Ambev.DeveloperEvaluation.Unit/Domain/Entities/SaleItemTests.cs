using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleItemTests
    {
        [Fact(DisplayName = "CancelItem should mark item as cancelled")]
        public void Given_SaleItem_When_Cancelled_Then_IsItemCancelledShouldBeTrue()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 5, discount: 0.1m);

            // act
            item.CancelItem();

            // assert
            Assert.True(item.IsItemCancelled);
        }

        [Fact(DisplayName = "Validation should pass for valid item with quantity < 4 and 0% discount")]
        public void Given_QuantityLessThan4AndNoDiscount_When_Validated_Then_Valid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 2, discount: 0);

            // act
            var result = item.Validate();

            // assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail if discount is 10% with quantity < 4")]
        public void Given_QuantityLessThan4AndDiscount10_When_Validated_Then_Invalid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 2, discount: 0.1m);

            // act
            var result = item.Validate();

            // assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Validation should pass for quantity between 4 and 9 and 10% discount")]
        public void Given_QuantityBetween4And9AndDiscount10_When_Validated_Then_Valid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 5, discount: 0.1m);

            // act
            var result = item.Validate();

            // assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail for quantity 4-9 with incorrect discount")]
        public void Given_Quantity5AndDiscount20_When_Validated_Then_Invalid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 5, discount: 0.2m);

            // act
            var result = item.Validate();

            // assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Validation should pass for quantity >= 10 and 20% discount")]
        public void Given_Quantity10AndDiscount20_When_Validated_Then_Valid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 10, discount: 0.2m);

            // act
            var result = item.Validate();

            // assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail for negative unit price")]
        public void Given_NegativeUnitPrice_When_Validated_Then_Invalid()
        {
            // arrange
            var item = SaleItemTestData.GenerateValidItem(quantity: 1, discount: 0);
            item.UnitPrice = -1;

            // act
            var result = item.Validate();

            // assert
            Assert.False(result.IsValid);
        }
    }
}
