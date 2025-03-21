using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class ProductTestData
{
    private static readonly Faker<Product> ProductFaker = new Faker<Product>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(1, 500))
        .RuleFor(p => p.IsActive, true)
        .RuleFor(p => p.CreatedAt, f => DateTime.UtcNow)
        .RuleFor(p => p.UpdatedAt, f => null);

    public static Product GenerateValidProduct() => ProductFaker.Generate();
}
