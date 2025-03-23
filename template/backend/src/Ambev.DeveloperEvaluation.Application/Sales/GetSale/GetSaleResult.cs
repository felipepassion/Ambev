using Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for GetSale operation
/// </summary>
public class GetSaleResult
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The total amount for the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates whether this sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Optional: The collection of sale items if you want to return them.
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = [];
}
