using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleTests
    {
        [Fact(DisplayName = "Cancel should mark sale as cancelled and update timestamp")]
        public void Given_Sale_When_Cancelled_Then_IsCancelledShouldBeTrue_And_UpdatedAtSet()
        {
            // arrange
            var sale = SaleTestData.GenerateValidSale();

            // act
            sale.Cancel();

            // assert
            Assert.True(sale.IsCancelled);
            Assert.NotNull(sale.UpdatedAt);
        }

        [Fact(DisplayName = "Validation should pass for valid sale")]
        public void Given_ValidSale_When_Validated_Then_ShouldBeValid()
        {
            // arrange
            var sale = SaleTestData.GenerateValidSale();

            // act
            var result = sale.Validate();

            // assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail if sale number is empty")]
        public void Given_EmptySaleNumber_When_Validated_Then_ShouldBeInvalid()
        {
            // arrange
            var sale = SaleTestData.GenerateValidSale();
            sale.SaleNumber = "";

            // act
            var result = sale.Validate();

            // assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail if total amount is negative")]
        public void Given_NegativeTotalAmount_When_Validated_Then_ShouldBeInvalid()
        {
            // arrange
            var sale = SaleTestData.GenerateValidSale();
            sale.Items.ForEach(x => x.TotalItemAmount = -100);

            // act
            var result = sale.Validate();

            // assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "Validation should fail if there are no sale items")]
        public void Given_SaleWithoutItems_When_Validated_Then_ShouldBeInvalid()
        {
            // arrange
            var sale = SaleTestData.GenerateValidSale();
            sale.Items.Clear();

            // act
            var result = sale.Validate();

            // assert
            Assert.False(result.IsValid);
        }
    }
}
