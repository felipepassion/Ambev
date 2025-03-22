namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents the response after successfully retrieving a sale.
/// </summary>
public class GetSaleResponse
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid SaleId { get; set; }

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
    public List<GetSaleItemResponse>? Items { get; set; }
}

/// <summary>
/// Sub-response for a single item in the sale.
/// </summary>
public class GetSaleItemResponse
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalItemAmount { get; set; }
}
