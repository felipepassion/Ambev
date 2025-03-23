namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

/// <summary>
/// Response model for GetProduct operation
/// </summary>
public class GetProductResult
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The optional description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The unit price for the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Indicates whether this product is active.
    /// </summary>
    public bool IsActive { get; set; }
}
