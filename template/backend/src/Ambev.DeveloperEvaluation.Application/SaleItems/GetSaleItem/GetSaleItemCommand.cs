using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;

/// <summary>
/// Command for retrieving a saleitem by their ID
/// </summary>
public record GetSaleItemCommand : IRequest<GetSaleItemResult>
{
    /// <summary>
    /// The unique identifier of the saleitem to retrieve
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of GetSaleItemCommand
    /// </summary>
    /// <param name="id">The ID of the saleitem to retrieve</param>
    public GetSaleItemCommand(Guid id)
    {
        Id = id;
    }
}
