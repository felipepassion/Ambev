using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests.
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mediator">The MediatR mediator instance.</param>
    public DeleteSaleHandler(
        ISaleRepository saleRepository,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request.
    /// </summary>
    /// <param name="request">The DeleteSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the delete operation.</returns>
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        // Publish the SaleDeletedEvent after successful deletion
        await _mediator.Publish(new SaleDeletedEvent(request.Id), cancellationToken);

        return new DeleteSaleResponse { Success = true };
    }
}
