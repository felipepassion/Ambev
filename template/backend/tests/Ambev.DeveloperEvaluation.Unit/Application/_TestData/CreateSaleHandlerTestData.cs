using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application._TestData;

/// <summary>
/// Provides methods for generating test data for the CreateSaleCommand.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand> _saleFaker =
        new Faker<CreateSaleCommand>()
            .RuleFor(cmd => cmd.BranchId, f => Guid.NewGuid()) // or f.Random.Guid()
            .RuleFor(cmd => cmd.Items, f =>
            {
                // Generate 1 to 3 random items
                var items = new List<CreateSaleItemCommand>();
                for (int i = 0; i < f.Random.Int(1, 3); i++)
                {
                    items.Add(new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = f.Random.Int(1, 5) // up to 5 for test
                    });
                }
                return items;
            });

    /// <summary>
    /// Generates a valid CreateSaleCommand with random data.
    /// </summary>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return _saleFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid CreateSaleCommand (e.g., empty branch and empty items).
    /// </summary>
    public static CreateSaleCommand GenerateInvalidCommand()
    {
        return new CreateSaleCommand
        {
            BranchId = Guid.Empty,
            Items = new List<CreateSaleItemCommand>() // No items => invalid
        };
    }
}
