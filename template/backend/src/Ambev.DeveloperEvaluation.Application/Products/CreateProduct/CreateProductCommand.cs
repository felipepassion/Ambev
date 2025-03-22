using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Command for creating a new Product.
    /// </summary>
    /// <remarks>
    /// This command captures data for product creation, including name, description,
    /// and unit price. It returns a <see cref="CreateProductResult"/>.
    /// </remarks>
    public class CreateProductCommand : IRequest<CreateProductResult>
    {
        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unit price for the product.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
