using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData
{
    /// <summary>
    /// Provides methods for generating test data (requests) for the SalesController.
    /// </summary>
    public static class SalesControllerTestData
    {
        private static readonly Faker<CreateSaleItemRequest> _saleItemFaker =
            new Faker<CreateSaleItemRequest>()
                .RuleFor(i => i.ProductId, f => Guid.NewGuid())
                .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

        private static readonly Faker<CreateSaleRequest> _saleFaker =
            new Faker<CreateSaleRequest>()
                .RuleFor(r => r.BranchId, f => Guid.NewGuid())
                .RuleFor(r => r.Items, f => _saleItemFaker.Generate(f.Random.Int(1, 3)));

        /// <summary>
        /// Generates a valid CreateSaleRequest with randomized data.
        /// </summary>
        public static CreateSaleRequest GenerateValidCreateSaleRequest()
        {
            return _saleFaker.Generate();
        }

        /// <summary>
        /// Generates an invalid CreateSaleRequest that fails validation
        /// (branch empty, no items, etc.).
        /// </summary>
        public static CreateSaleRequest GenerateInvalidCreateSaleRequest()
        {
            return new CreateSaleRequest
            {
                BranchId = Guid.Empty,      // triggers invalid
                Items = new List<CreateSaleItemRequest>() // triggers invalid
            };
        }
    }
}
