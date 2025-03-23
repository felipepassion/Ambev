using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData;

public static class SalesIntegrationTestData
{
    private static readonly Faker<CreateSaleRequest> _saleFaker = new Faker<CreateSaleRequest>()
        .RuleFor(r => r.BranchId, f => f.Random.Guid()) // a a a
        .RuleFor(r => r.Items, f => new List<CreateSaleItemRequest>
        {
            new CreateSaleItemRequest
            {
                ProductId = f.Random.Guid(), // a a a
                Quantity = f.Random.Int(1, 10) // a a a
            }
        });

    public static CreateSaleRequest GenerateValidCreateSaleRequest()
    {
        return _saleFaker.Generate();
    }

    public static CreateSaleRequest GenerateInvalidCreateSaleRequest()
    {
        return new CreateSaleRequest
        {
            BranchId = Guid.Empty, // a a a
            Items = new List<CreateSaleItemRequest>() // a a a
        };
    }

}
