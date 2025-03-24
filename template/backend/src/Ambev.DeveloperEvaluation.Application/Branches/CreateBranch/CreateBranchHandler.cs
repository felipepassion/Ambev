using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;

/// <summary>
/// Handler for processing CreateBranchCommand requests.
/// </summary>
public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, CreateBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateBranchHandler(IBranchRepository branchRepository, IMapper mapper, IMediator mediator)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateBranchResult> Handle(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingBranch = await _branchRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingBranch != null)
            throw new InvalidOperationException($"Branch with name {command.Name} already exists");

        var branch = _mapper.Map<Branch>(command);
        var createdBranch = await _branchRepository.CreateAsync(branch, cancellationToken);
        var result = _mapper.Map<CreateBranchResult>(createdBranch);

        await _mediator.Publish(new BranchCreatedEvent(createdBranch.Id, createdBranch.Name), cancellationToken);

        return result;
    }
}
