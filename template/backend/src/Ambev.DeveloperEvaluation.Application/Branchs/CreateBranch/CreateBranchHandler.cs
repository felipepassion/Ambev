using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branchs.CreateBranch;

/// <summary>
/// Handler for processing CreateBranchCommand requests
/// </summary>
public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, CreateBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateBranchHandler
    /// </summary>
    /// <param name="branchRepository">The branch repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="validator">The validator for CreateBranchCommand</param>
    public CreateBranchHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateBranchCommand request
    /// </summary>
    /// <param name="command">The CreateBranch command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created branch details</returns>
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

        return result;
    }
}
