using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData;

/// <summary>
/// Provides methods for generating test data (requests) for the ProductsController.
/// </summary>
public static class ProductsControllerTestData
{
    private static readonly Faker<CreateProductRequest> _createProductFaker =
        new Faker<CreateProductRequest>()
            .RuleFor(r => r.Name, f => f.Commerce.ProductName())
            .RuleFor(r => r.Description, f => f.Commerce.ProductDescription())
            .RuleFor(r => r.UnitPrice, f => f.Random.Decimal(1, 999));

    /// <summary>
    /// Generates a valid CreateProductRequest with randomized data.
    /// </summary>
    public static CreateProductRequest GenerateValidCreateProductRequest()
    {
        return _createProductFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid CreateProductRequest that fails validation.
    /// </summary>
    public static CreateProductRequest GenerateInvalidCreateProductRequest()
    {
        return new CreateProductRequest
        {
            Name = "",          // Required => invalid
            Description = new string('x', 600), // Exceeding 500 chars (if there's a rule)
            UnitPrice = -10m    // Negative => invalid
        };
    }
}
