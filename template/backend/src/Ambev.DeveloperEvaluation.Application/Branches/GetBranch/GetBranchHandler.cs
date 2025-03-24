using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.GetBranch;

/// <summary>
/// Handler for processing GetBranchCommand requests.
/// Validates the command, retrieves the branch, and publishes a BranchRetrievedEvent.
/// </summary>
public class GetBranchHandler : IRequestHandler<GetBranchCommand, GetBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetBranchHandler(IBranchRepository branchRepository, IMapper mapper, IMediator mediator)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<GetBranchResult> Handle(GetBranchCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetBranchValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = await _branchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (branch == null)
            throw new KeyNotFoundException($"Branch with ID {request.Id} not found");

        var result = _mapper.Map<GetBranchResult>(branch);

        // Publish BranchRetrievedEvent after successful retrieval
        await _mediator.Publish(new BranchRetrievedEvent(branch.Id), cancellationToken);

        return result;
    }
}
