namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Response returned after successfully creating a sale.
/// </summary>
public class CreateSaleResponse
{
    /// <summary>
    /// The unique identifier of the newly created sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The total amount for this sale, after any discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }
}
