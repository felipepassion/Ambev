namespace Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;

/// <summary>
/// Response model for GetSaleItem operation
/// </summary>
public class GetSaleItemResult
{
    /// <summary>
    /// The unique identifier of the saleitem
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The saleitem's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The saleitem's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The saleitem's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}
