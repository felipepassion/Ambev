using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

namespace Ambev.DeveloperEvaluation.Integration.TestsData;

public static class ProductsIntegrationTestData
{
    private static readonly Faker<CreateProductRequest> _productFaker =
        new Faker<CreateProductRequest>()
            .RuleFor(r => r.Name, f => f.Commerce.ProductName())
            .RuleFor(r => r.Description, f => f.Commerce.ProductDescription())
            .RuleFor(r => r.UnitPrice, f => f.Random.Decimal(1, 999));

    public static CreateProductRequest GenerateValidCreateProductRequest()
    {
        return _productFaker.Generate();
    }

    public static CreateProductRequest GenerateInvalidCreateProductRequest()
    {
        return new CreateProductRequest
        {
            Name = "",
            Description = new string('x', 600),
            UnitPrice = -10m
        };
    }
}
