namespace Ambev.DeveloperEvaluation.Application.SaleItems.CreateSaleItem;

/// <summary>
/// Represents the response returned after successfully creating a new saleitem.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly created saleitem,
/// which can be used for subsequent operations or reference.
/// </remarks>
public class CreateSaleItemResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created saleitem.
    /// </summary>
    /// <value>A GUID that uniquely identifies the created saleitem in the system.</value>
    public Guid Id { get; set; }
}
