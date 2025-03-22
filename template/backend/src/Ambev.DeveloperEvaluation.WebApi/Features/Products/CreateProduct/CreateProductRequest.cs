namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Request object for creating a new product.
/// </summary>
public class CreateProductRequest
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// An optional description for the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The unit price for this product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
