namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for GetSale operation
/// </summary>
public class GetSaleResult
{
    /// <summary>
    /// The unique identifier of the sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sale's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The sale's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The sale's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}
