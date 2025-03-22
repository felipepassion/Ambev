using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Cancel should mark sale as cancelled and update timestamp")]
    public void Given_Sale_When_Cancelled_Then_IsCancelledShouldBeTrue_And_UpdatedAtSet()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.NotNull(sale.UpdatedAt);
    }

    [Fact(DisplayName = "Validation should pass for valid sale")]
    public void Given_ValidSale_When_Validated_Then_ShouldBeValid()
    {
        var sale = SaleTestData.GenerateValidSale();

        var result = sale.Validate();

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validation should fail if sale number is empty")]
    public void Given_EmptySaleNumber_When_Validated_Then_ShouldBeInvalid()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.SaleNumber = "";

        var result = sale.Validate();

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Validation should fail if total amount is negative")]
    public void Given_NegativeTotalAmount_When_Validated_Then_ShouldBeInvalid()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.ForEach(x=> x.TotalItemAmount = -100);

        var result = sale.Validate();

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Validation should fail if there are no sale items")]
    public void Given_SaleWithoutItems_When_Validated_Then_ShouldBeInvalid()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.Clear();

        var result = sale.Validate();

        Assert.False(result.IsValid);
    }
}
