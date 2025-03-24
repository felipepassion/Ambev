using Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.Domain.Events.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Unit.Application.Branches;

/// <summary>
/// Handler for processing DeleteBranchCommand requests.
/// Validates the command, deletes the branch, and publishes a BranchDeletedEvent.
/// </summary>
public class DeleteBranchHandler : IRequestHandler<DeleteBranchCommand, DeleteBranchResponse>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMediator _mediator;

    public DeleteBranchHandler(IBranchRepository branchRepository, IMediator mediator)
    {
        _branchRepository = branchRepository;
        _mediator = mediator;
    }

    public async Task<DeleteBranchResponse> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteBranchValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _branchRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Branch with ID {request.Id} not found");

        await _mediator.Publish(new BranchDeletedEvent(request.Id), cancellationToken);

        return new DeleteBranchResponse { Success = true };
    }
}
