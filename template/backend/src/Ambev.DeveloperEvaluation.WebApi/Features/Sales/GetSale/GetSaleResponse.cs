using System.Text.Json.Serialization;

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
    /// Gets or sets the user identifier associated with the sale.
    /// </summary>
    [JsonPropertyName("customerId")]
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public required string SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale occurred.
    /// </summary>
    public required DateTime SaleDate { get; set; }

    /// <summary>
    /// Optional: The collection of sale items if you want to return them.
    /// </summary>
    public List<GetSaleItemResponse>? Items { get; set; }
}