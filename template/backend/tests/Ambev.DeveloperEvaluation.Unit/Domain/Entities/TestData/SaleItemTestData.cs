using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    public static SaleItem GenerateValidItem(int quantity, decimal discount)
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SaleId = Guid.NewGuid(),
            Quantity = quantity,
            UnitPrice = 10.0m,
            Discount = discount,
            TotalItemAmount = 0,
            IsItemCancelled = false
        };
    }
}
