namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents the response after successfully retrieving a sale.
/// </summary>
public class GetSaleResponse
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
    public List<GetSaleItemResponse>? Items { get; set; }
}