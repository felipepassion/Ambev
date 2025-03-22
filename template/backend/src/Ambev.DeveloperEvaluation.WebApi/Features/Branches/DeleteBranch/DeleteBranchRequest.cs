namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.DeleteBranch;

/// <summary>
/// Request model for deleting a branch
/// </summary>
public class DeleteBranchRequest
{
    /// <summary>
    /// The unique identifier of the branch to delete
    /// </summary>
    public Guid Id { get; set; }
}
