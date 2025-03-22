namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Request object for creating a new sale.
/// </summary>
/// <remarks>
/// Contains the branch identifier and a list of items for the sale.
/// </remarks>
public class CreateSaleRequest
{
    /// <summary>
    /// The branch where the sale is happening.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The list of items (products + quantity) included in this sale.
    /// </summary>
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Sub-request representing a single item in the sale.
/// </summary>
public class CreateSaleItemRequest
{
    /// <summary>
    /// The product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// How many units of this product are purchased in this sale.
    /// </summary>
    public int Quantity { get; set; }
}
