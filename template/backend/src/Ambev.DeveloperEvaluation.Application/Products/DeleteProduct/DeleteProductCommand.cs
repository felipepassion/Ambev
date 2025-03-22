using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

/// <summary>
/// Command for deleting a product
/// </summary>
public record DeleteProductCommand : IRequest<DeleteProductResponse>
{
    /// <summary>
    /// The unique identifier of the product to delete
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of DeleteProductCommand
    /// </summary>
    /// <param name="id">The ID of the product to delete</param>
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}
