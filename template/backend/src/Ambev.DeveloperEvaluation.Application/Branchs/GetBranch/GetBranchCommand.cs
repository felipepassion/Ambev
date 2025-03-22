using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branchs.GetBranch;

/// <summary>
/// Command for retrieving a branch by their ID
/// </summary>
public record GetBranchCommand : IRequest<GetBranchResult>
{
    /// <summary>
    /// The unique identifier of the branch to retrieve
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of GetBranchCommand
    /// </summary>
    /// <param name="id">The ID of the branch to retrieve</param>
    public GetBranchCommand(Guid id)
    {
        Id = id;
    }
}
