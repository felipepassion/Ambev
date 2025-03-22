using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.SaleItems.DeleteSaleItem;

/// <summary>
/// Handler for processing DeleteSaleItemCommand requests
/// </summary>
public class DeleteSaleItemHandler : IRequestHandler<DeleteSaleItemCommand, DeleteSaleItemResponse>
{
    private readonly ISaleItemRepository _saleitemRepository;

    /// <summary>
    /// Initializes a new instance of DeleteSaleItemHandler
    /// </summary>
    /// <param name="saleitemRepository">The saleitem repository</param>
    /// <param name="validator">The validator for DeleteSaleItemCommand</param>
    public DeleteSaleItemHandler(
        ISaleItemRepository saleitemRepository)
    {
        _saleitemRepository = saleitemRepository;
    }

    /// <summary>
    /// Handles the DeleteSaleItemCommand request
    /// </summary>
    /// <param name="request">The DeleteSaleItem command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleItemResponse> Handle(DeleteSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _saleitemRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"SaleItem with ID {request.Id} not found");

        return new DeleteSaleItemResponse { Success = true };
    }
}
