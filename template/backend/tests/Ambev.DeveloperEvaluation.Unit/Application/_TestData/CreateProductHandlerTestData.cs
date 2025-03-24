using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application._TestData;

/// <summary>
/// Provides methods for generating test data for CreateProductCommand.
/// </summary>
public static class CreateProductHandlerTestData
{
    private static readonly Faker<CreateProductCommand> _productFaker =
        new Faker<CreateProductCommand>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(1, 1000));

    public static CreateProductCommand GenerateValidCommand()
    {
        return _productFaker.Generate();
    }

    public static CreateProductCommand GenerateInvalidCommand()
    {
        return new CreateProductCommand
        {
            Name = "", // invalid: required
            UnitPrice = -10 // invalid: must be >= 0
        };
    }
}
