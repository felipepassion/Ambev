using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;

/// <summary>
/// Command for deleting a branch
/// </summary>
public record DeleteBranchCommand : IRequest<DeleteBranchResponse>
{
    /// <summary>
    /// The unique identifier of the branch to delete
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of DeleteBranchCommand
    /// </summary>
    /// <param name="id">The ID of the branch to delete</param>
    public DeleteBranchCommand(Guid id)
    {
        Id = id;
    }
}
