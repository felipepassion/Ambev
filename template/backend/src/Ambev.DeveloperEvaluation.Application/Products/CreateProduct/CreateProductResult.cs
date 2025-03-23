namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

/// <summary>
/// Represents the response returned after successfully creating a new product.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly created product,
/// which can be used for subsequent operations or reference.
/// </remarks>
public class CreateProductResult
{
    /// <summary>
    /// The unique identifier of the newly created product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the newly created product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The optional description of the newly created product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The unit price for the newly created product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
