namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for creating a new saleitem.
/// </summary>
/// <remarks>
public class CreateSaleItemCommand
{
    /// <summary>
    /// Gets or sets the product identifier for this sale item.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this product to be purchased.
    /// </summary>
    public int Quantity { get; set; }
}