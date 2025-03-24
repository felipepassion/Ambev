namespace Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;

/// <summary>
/// Response model for GetSaleItem operation
/// </summary>
public class GetSaleItemResult
{
    /// <summary>
    /// Represents the unique identifier for a product. It is of type Guid, ensuring a globally unique value.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Represents the quantity of an item. It can be both retrieved and modified.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Represents the discount amount as a decimal value. It can be both retrieved and set.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Represents the total amount of all items. It is a decimal value that can be used for financial calculations.
    /// </summary>
    public decimal TotalItemAmount { get; set; }
}
